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
        _labsContext.CurrentLabType = (LabType)type;
        if (!Data.LabNames.TryGetValue(_labsContext.CurrentLabType, out var value)) return NotFound(); 
        ViewData["Header"] = value.ToUpper();
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(InputModel model, bool IsDecryptPressed)
    {
        if (!ModelState.IsValid) return View();
        var encryptor = EncryptorBase.GetEncryptor(_labsContext.CurrentLabType, model.Key);
        model.Output = IsDecryptPressed ? encryptor.Decrypt(model.Input) : encryptor.Encrypt(model.Input);
        return Json(model);
    }
}