using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string UserPhoto { get; set; }
		public bool IsEnabled { get; set; }
		public string EmployeeId { get; set; }
	}
}
