using Laba1.Encryptors;
using Laba1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Laba1.Controllers;

public class Laba2Controller : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewData["Title"] = "Laba2";
        ViewBag.EncryptionMethod = "Метод перестановки".ToUpper();
        return View();
    }
    
    [HttpPost]
    public IActionResult Index(InputModel model, bool isDecryptFunc)
    {
        if (!ModelState.IsValid) return View();
        var encryptor = new PermutationEncryptor(model.Key);
        model.Output = isDecryptFunc ? encryptor.Decrypt(model.Input) : encryptor.Encrypt(model.Input);
        return Json(model);
    }
}