using Laba1.Encryptors;
using Laba1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laba1.Controllers;

public class Laba1Controller : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Laba1";
        ViewBag.EncryptionMethod = "Метод подстановки".ToUpper();
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(InputModel model, bool isDecryptFunc)
    {
        if (!ModelState.IsValid) return View("Index");
        var encryptor = new SubstitutionEncryptor(model.Key);
        model.Output = isDecryptFunc ? encryptor.Decrypt(model.Input) : encryptor.Encrypt(model.Input);
        return Json(model);
    }
}