﻿namespace SecureShop.Models
{
    public class ManageRolesViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public IList<string> Roles { get; set; }
    }
}
