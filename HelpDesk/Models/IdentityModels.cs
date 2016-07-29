
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HelpDesk.Models
{
    //public class ApplicationUserLogin : IdentityUserLogin { }
    //public class ApplicationUserClaim : IdentityUserClaim { }
    //public class ApplicationUserRole : IdentityUserRole {

    //    [Key]
    //    public string RoleId { get; set; }
    //}

    //public class ApplicationRole : IdentityRole
    //{
    //    public ApplicationRole() : base() { }
    //    public ApplicationRole(string name)
    //        : this()
    //    {
    //        RoleName = name;
    //        base.Name = name;
    //    }
    //    public string  RoleName { get; set; }
    //}


    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager
                .CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public Boolean IsAuthenticated { get; set; }

        //TODO Configure navigation properties for ApplicationUsers
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ICollection<Issue> Issues { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ICollection<Solution> Solutions { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public virtual ICollection<Activity> Activities { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonIgnore]
        public virtual ICollection<Step> Steps { get; set; }
    }

    public class ApplicationUserEditModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Boolean IsAuthenticated { get; set; }
    }

    //public class ApplicationUserStore :
    //UserStore<ApplicationUser>, IUserStore<ApplicationUser>, IDisposable
    //{
    //    public ApplicationUserStore()
    //        : this(new IdentityDbContext())
    //    {
    //        base.DisposeContext = true;
    //    }

    //    public ApplicationUserStore(DbContext context)
    //        : base(context)
    //    {
    //    }
    //}


    //public class ApplicationRoleStore
    //: RoleStore<ApplicationRole>,
    //IRoleStore<ApplicationRole>, IDisposable
    //{
    //    public ApplicationRoleStore()
    //        : base(new IdentityDbContext())
    //    {
    //        base.Dispose(true);
    //    }

    //    public ApplicationRoleStore(DbContext context)
    //        : base(context)
    //    {
    //    }
    //}
}

