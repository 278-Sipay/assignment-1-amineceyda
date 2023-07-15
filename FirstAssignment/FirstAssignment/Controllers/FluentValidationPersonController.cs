using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace FirstAssignment.Controllers
{
    // Represents a person object
    public class Person
    {
        [DisplayName("Staff person name")]
        public string Name { get; set; }

        [DisplayName("Staff person lastname")]
        public string Lastname { get; set; }

        [DisplayName("Staff person phone number")]
        public string Phone { get; set; }

        [DisplayName("Staff person access level to system")]
        public int AccessLevel { get; set; }

        [DisplayName("Staff person salary")]
        public decimal Salary { get; set; }
    }

    // the validator for the Person class
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            // Validation rules for the Name property
            RuleFor(p => p.Name)
                .NotEmpty() // Name should not be empty
                .WithMessage("Staff person name is required.")
                .Length(5, 100) // Name should have a length between 5 and 100 characters
                .WithMessage("Staff person name must be between 5 and 100 characters.");

            // Validation rules for the Lastname property
            RuleFor(p => p.Lastname)
                .NotEmpty() // Lastname should not be empty
                .WithMessage("Staff person lastname is required.")
                .Length(5, 100) // Lastname should have a length between 5 and 100 characters
                .WithMessage("Staff person lastname must be between 5 and 100 characters.");

            // Validation rules for the Phone property
            RuleFor(p => p.Phone)
                .NotEmpty() // Phone should not be empty
                .WithMessage("Staff person phone number is required.")
                .Matches(@"^\d{10}$") // Phone should match the pattern for a 10-digit number
                .WithMessage("Staff person phone number must be a 10-digit number.");

            // Validation rules for the AccessLevel property
            RuleFor(p => p.AccessLevel)
                .InclusiveBetween(1, 5) // AccessLevel should be between 1 and 5 (inclusive)
                .WithMessage("Staff person access level must be between 1 and 5.");

            // Validation rules for the Salary property
            RuleFor(p => p.Salary)
                .Must((person, salary) => IsValidSalary(person.AccessLevel, salary)) // Custom validation based on AccessLevel
                .WithMessage("Invalid salary for the staff person's access level.");
        }

        // Custom method to validate the Salary based on the AccessLevel
        private bool IsValidSalary(int accessLevel, decimal salary)
        {
            // Salary should not exceed the allowed limit based on the AccessLevel
            switch (accessLevel)
            {
                case 1:
                    return salary <= 10000;
                case 2:
                    return salary <= 20000;
                case 3:
                    return salary <= 30000;
                case 4:
                    return salary <= 40000;
                default:
                    return false;
            }
        }
    }


    [Route("sipy/api/[controller]")]
    [ApiController]
    public class FluentValidationPersonController : ControllerBase
    {
        public FluentValidationPersonController() { }

        [HttpPost]
        public ActionResult<Person> Post([FromBody] Person person)
        {
            var validator = new PersonValidator();
            ValidationResult result = validator.Validate(person);

            if (!result.IsValid)
            {
                // Add validation errors to the ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return BadRequest(ModelState);
            }

            return person;
        }

    }
}
