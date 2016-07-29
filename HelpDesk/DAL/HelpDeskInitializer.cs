using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HelpDesk.DAL
{
    public class HelpDeskInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<HelpDeskContext>
    {
        
    }
}