﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Ark.AspNetCore.CommaSeparatedParameters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class SeparatedQueryValueAttribute : Attribute, IResourceFilter
    {
        private readonly SeparatedQueryValueProviderFactory _factory;

        public SeparatedQueryValueAttribute() : this(',')
        {
        }

        public SeparatedQueryValueAttribute(char separator) : this(null, separator)
        {
        }

        public SeparatedQueryValueAttribute(string key, char separator)
        {
            _factory = new SeparatedQueryValueProviderFactory(key, separator);
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            context.ValueProviderFactories.Insert(0, _factory);
        }
    }
}