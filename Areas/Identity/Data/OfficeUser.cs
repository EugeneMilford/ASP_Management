using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OfficeManagement.Areas.Identity.Data;

// Add profile data for application users by adding properties to the OfficeUser class
public class OfficeUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
    public string UserRole { get; set; }
}

