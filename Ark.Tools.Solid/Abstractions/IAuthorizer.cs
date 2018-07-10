﻿// Copyright (c) 2018 Ark S.r.l. All rights reserved.
// Licensed under the MIT License. See LICENSE file for license information. 
namespace Ark.Tools.Solid.Abstractions

{
    public interface IAuthorizer<T>
    {
        void AuthorizeOrThrow(T dto);
    }
}
