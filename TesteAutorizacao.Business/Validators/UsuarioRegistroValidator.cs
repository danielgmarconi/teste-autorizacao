using FluentValidation;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteAutorizacao.Business.Repository;
using TesteAutorizacao.Model;

namespace VerisUserAuthentication.Business.Validators
{
    public class UsuarioRegistroValidator : AbstractValidator<Usuario>
    {

        public UsuarioRegistroValidator(IUsuarioRepository userRepository)
        {
                RuleFor(t => t.Email).NotEmpty().WithMessage("{PropertyName} " + "não pode ser nulo.")
                                     .EmailAddress().WithMessage("{PropertyName} " + "invalido");
                RuleFor(t => t.Senha).NotEmpty().WithMessage("{PropertyName} " + "não pode ser nulo.")
                                        .MinimumLength(5).WithMessage("{PropertyName} " + "não pode ter menos 8 caracteres.")
                                        .Matches(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$").WithMessage("{PropertyName} " + "deve conter caracteres especial, letras maiúsculas, letras minúsculas e números.");
                RuleFor(t => t).Must(t => !userRepository.IsExists(new Usuario() { Email = t.Email })).WithMessage("Já esta cadastrado");
        }
    }
}
