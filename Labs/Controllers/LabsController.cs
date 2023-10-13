using System.Diagnostics;
using System.Text.RegularExpressions;
using Laba1.Models;
using lib.Labs;
using lib.Labs.Encryptors;
using Microsoft.AspNetCore.Mvc;

namespace Laba1.Controllers;

public class LabsController : Controller
{
    private readonly ILabsContext _labsContext;
    
    public LabsController(ILabsContext labsContext)
    {
        _labsContext = labsContext;
    }
    
    [HttpGet]
    public IActionResult Index([FromQuery]int type = 0)
    {
        _labsContext.LabType = (LabType)type;
        ViewData["Header"] = _labsContext.LabProperties.Name.ToUpper();
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(InputModel model, bool IsDecryptPressed)
    {
        if (!ModelState.IsValid) return View();
        if (model.Key != null && !Regex.IsMatch(model.Key, _labsContext.LabProperties.KeyPattern)) 
            return Json(new { Error = "Ключ содержит недопустимые символы"});
        
        var encryptor = EncryptorBase.GetEncryptor(_labsContext.LabType, model.Key, 
            _labsContext.LabProperties.Alphabet);
        if (encryptor == null) return Json(new {Error = "Шифратор не загружен"});
        
        if (!encryptor.ValidateInput(model.Input)) return Json(new {Error = "Ввод не соответствует алфавиту"});
        
        model.Output = IsDecryptPressed ? encryptor.Decrypt(model.Input) : encryptor.Encrypt(model.Input);
        return Json(model);
    }
}