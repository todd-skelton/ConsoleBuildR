using ConsoleBuildR;
using EfCoreApp.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfCoreApp
{
    public class AddUsersFromConfiguration : IExecutable
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _applicationDbContext;

        public AddUsersFromConfiguration(IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _configuration = configuration;
            _applicationDbContext = applicationDbContext;
        }

        public async Task Execute(string[] args)
        {
            // pull users from appsettings.json
            var users = _configuration.GetSection("Users").Get<IEnumerable<User>>();

            // add users to database
            _applicationDbContext.Users.AddRange(users);

            // save changes
            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
