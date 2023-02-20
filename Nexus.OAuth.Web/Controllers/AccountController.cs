﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Newtonsoft.Json;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
using Nexus.OAuth.Web.Models.Enums;
using Nexus.OAuth.Web.Models.Responses;
using Nexus.Tools.Validations.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Net;
using EmailAddressAttribute = Nexus.Tools.Validations.Attributes.EmailAddressAttribute;
using PhoneAttribute = Nexus.Tools.Validations.Attributes.PhoneAttribute;
using RequiredAttribute = Nexus.Tools.Validations.Attributes.RequiredAttribute;
using StringLengthAttribute = Nexus.Tools.Validations.Attributes.StringLengthAttribute;

namespace Nexus.OAuth.Web.Controllers;

public partial class AccountController : BaseController
{
    private readonly ILogger<AccountController> _logger;
    private readonly string hCaptchaKey;
    public AccountController(IConfiguration config, ILogger<AccountController> logger)
    {
        _logger = logger;
        hCaptchaKey = config.GetSection("hCaptcha:SiteKey").Get<string>() ?? string.Empty;
    }

    public IActionResult ConfirmationModal()
        => View();
    public IActionResult Register(string? after)
    {
        after ??= DefaultRedirect;

        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after;
        ViewBag.hCaptchaKey = hCaptchaKey;
        return View();
    }

    public IActionResult Recovery(string? after)
    {
        after ??= DefaultRedirect;

        if (XssValidation(ref after))
            return XssError();

        ViewBag.RedirectTo = after;
        return View();
    }
    public IActionResult Confirm(ConfirmationType type, string token)
    {
        token ??= string.Empty;
        if (XssValidation(ref token))
            return XssError();

        ViewBag.Token = token;
        ViewBag.Type = Enum.GetName(type);
        return View();
    }

    #region Register
    [HttpPost]
    public IActionResult RegisterChat(string? input, RegisterStep step)
    {
        IEnumerable<ValidationResult> errors;
        RegisterStep nextStep = step + 1;

        if (string.IsNullOrEmpty(input) && step != RegisterStep.Welcome)
            return Text(step, HttpStatusCode.BadRequest, "O valor de entrada não pode ser nulo!");

        if (XssValidation(ref input))
            return XssError();

        switch (step)
        {
            case RegisterStep.Welcome:
                return Text(nextStep,
                    text: "Seja bem-vindo a Nexus Company, para continuar digite seu nome completo: ",
                    placeholder: "João Pereira Santos.");
            case RegisterStep.Name:
                errors = ValidarEntrada(input,
                    new ValidationAttribute[] {
                    new NameAttribute(),
                    new RequiredAttribute(),
                    new StringLengthAttribute(50){ MinimumLength = 3} });

                if (errors.Any())
                    return BadRequest(errors);

                string[] names = input.Split(' ');
                return Text(nextStep,
                    text: $"Bem vindo, {names[0]} {names[1]}! Continue seu cadastro digitando seu e-mail:",
                    placeholder: "conta@example.com",
                    type: "email");
            case RegisterStep.Email:
                errors = ValidarEntrada(input,
                  new ValidationAttribute[] {
                        new RequiredAttribute(),
                        new EmailAddressAttribute(),
                        new StringLengthAttribute(500){ MinimumLength = 5} });

                if (errors.Any())
                    return BadRequest(errors);

                return Text(nextStep,
                    text: $"Ok, agora vamos precisar do seu numero de telefone:",
                    placeholder: "+55 (00) 99999-9999",
                    type: "phone");
            case RegisterStep.Phone:
                errors = ValidarEntrada(input, new ValidationAttribute[] {
                        new PhoneAttribute(),
                        new RequiredAttribute(),
                        new StringLengthAttribute(21){ MinimumLength = 5} });

                if (errors.Any())
                    return BadRequest(errors);

                return Text(nextStep,
                    text: "Beleza, precisamos saber só mais um pouco sobre você por isso entre com sua data de nascimento:",
                    placeholder: "00/00/0000",
                    type: "date");
            case RegisterStep.DateOfBirth:
                errors = ValidarEntrada(input, new ValidationAttribute[] {
                        new RequiredAttribute() });

                if (!DateTime.TryParse(input, out DateTime date))
                    errors = errors.Append(new ValidationResult("Use o formato correto para datas MM/DD/YYYY", new string[] { "Birthday" }));

                if ((date.Year - DateTime.UtcNow.Year) > 90)
                    errors = errors.Append(new ValidationResult("Isto é sério ?", new string[] { Enum.GetName(RegisterStep.DateOfBirth) ?? "DateOfBirth" }));

                if (errors.Any())
                    return BadRequest(errors);

                return Text(nextStep,
                    text: "Vamos criar uma senha para a senha conta, lembre-se de anota-lá é nunca compartilhar: ",
                    placeholder: "******",
                    type: "password");
            case RegisterStep.Password:
                errors = ValidarEntrada(input, new ValidationAttribute[] {
                        new PasswordAttribute(),
                        new RequiredAttribute(),
                        new StringLengthAttribute(21){ MinimumLength = 5} });

                if (errors.Any())
                    return BadRequest(errors);

                return Text(nextStep,
                    text: "Agora vamos confirmar sua senha, por favor digite-a novamente: ",
                    placeholder: "******",
                    type: "password");
            case RegisterStep.ConfirmPassword:
                string[] arr = input?.Split('\0');
                errors = Array.Empty<ValidationResult>();

                if (arr.Length != 2)
                    errors = errors.Append(new ValidationResult(nameof(Account.ConfirmPassword), new string[] { "Adicione a senha é a senha de confirmação separados por um caracter nulo." }));

                if (arr[0] != arr[1])
                    errors = errors.Append(new ValidationResult(nameof(Account.ConfirmPassword), new string[] { "" }));

                if (errors.Any())
                    return BadRequest(errors);

                return Text(nextStep,
                                   text: "Para finalizar aceite os termos e politica de privacidade! Caso não tenha aberto o modal <a class=\"trm\" onclick=\"showTerms()\"> Clique Aqui </a>.",
                                   placeholder: "******",
                                   type: "password");
            case RegisterStep.Terms:
                arr = input?.Split('\0');
                errors = Array.Empty<ValidationResult>();

                if (arr.Length != 2)
                    errors = errors.Append(new ValidationResult(nameof(Account.ConfirmPassword), new string[] { "Adicione a senha é a senha de confirmação separados por um caracter nulo." }));

                if (!bool.TryParse(arr[0], out bool termsNexus) || !termsNexus)
                    errors = errors.Append(new ValidationResult(nameof(Account.AcceptTerms), new string[] { "Você deve aceita os termos da Nexus para continuar seu cadastro." }));

                if (!bool.TryParse(arr[1], out bool termsCaptcha) || !termsCaptcha)
                    errors = errors.Append(new ValidationResult("AcceptTermsCaptcha", new string[] { "Você deve aceita os termos de hCaptcha para continuar seu cadastro." }));
               
                if (errors.Any())
                    return BadRequest(errors);

                return StatusCode((int)HttpStatusCode.Continue);
        }

        throw new NotImplementedException();
    }

    public IEnumerable<ValidationResult> ValidarEntrada(object entrada, IEnumerable<ValidationAttribute> attrs)
    {
        var validationContext = new ValidationContext(entrada, serviceProvider: null, items: null);
        var validationResults = new List<ValidationResult>();

        Validator.TryValidateValue(entrada, validationContext, validationResults, attrs);

        return validationResults;
    }

    private IActionResult Text(RegisterStep nextStep,
        HttpStatusCode status = HttpStatusCode.OK,
        string? text = null,
        string placeholder = "Insira um texto aqui!",
        string type = "text")
        => Ok(new ChatResponse((int)status, nextStep, text, placeholder, type));
    #endregion
}