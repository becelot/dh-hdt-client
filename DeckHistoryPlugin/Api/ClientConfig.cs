﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckHistoryPlugin.Api
{
    class ClientConfig
    {
        private const string Base = "http://localhost:5000/api";

        private const string DefaultUserLoginUrl = Base + "/auth/login";

        public string UserLoginUrl { get; set; } = DefaultUserLoginUrl;
    }
}
