using System.Text.RegularExpressions;
using Labs.Models;
using lib.Labs;
using Microsoft.AspNetCore.Mvc;
using lib.Labs.Encryptors;
using lib.Labs.Encryptors.Interfaces;

namespace Labs.Controllers;

public class LabsController : Controller
{
    private readonly ILabsContext _labsContext;

    public LabsController(ILabsContext labsContext)
    {
        _labsContext = labsContext;
    }

    [HttpGet]
    public IActionResult Index()
    {
        _labsContext.LabType = Enum.Parse<LabType>(GetQueryStringValue(Request.QueryString.Value, "type") ?? LabType.Lab1.ToString());
        ViewData["Header"] = _labsContext.LabProperties.Name.ToUpper();
        ViewBag.CanGenerateKey = EncryptorBase.GetEncryptor(_labsContext.LabType) is IKeyGenerative;
        return View();
    }

    [HttpPost]
    public IActionResult Index(InputModel model, bool IsDecryptPressed)
    {
        if (!ModelState.IsValid) return View();
        if (model.Key != null &&
            (model.Key.Length < _labsContext.LabProperties.KeyLength
             || !Regex.IsMatch(model.Key, _labsContext.LabProperties.KeyPattern)))
            return Json(new { Error = "Ключ содержит недопустимые символы или меньше необходимой длины" });

        var encryptor = EncryptorBase.GetEncryptor(_labsContext.LabType, model.Key?.Replace(" ", string.Empty),
            _labsContext.LabProperties.Alphabet);
        if (encryptor == null) return Json(new { Error = "Шифратор не загружен" });
        
        if (!encryptor.ValidateInput(model.Input)) return Json(new { Error = "Ввод не соответствует алфавиту" });

        model.Output = IsDecryptPressed ? encryptor.Decrypt(model.Input) : encryptor.Encrypt(model.Input);
        return Json(model);
    }
    
    [HttpPost]
    public string GenerateKey()
    {
        return EncryptorBase.GetEncryptor(_labsContext.LabType) is IKeyGenerative enc
            ? enc.GenerateKey()
            : string.Empty;
    }

    private string GetQueryStringValue(string query, string param)
    {
        var res = query.Contains(param) ? query
            .Split('&')
            .FirstOrDefault(w => w.Contains(param))
            : null;
        return res?.Substring(res.IndexOf('=') + 1);
    }
}