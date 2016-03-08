// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BasicApi
{
    public class BaseController
    {
        [ActionContext]
        public ActionContext ActionContext { get; set; }

        public ModelStateDictionary ModelState => ActionContext.ModelState;
    }
}
