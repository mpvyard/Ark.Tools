﻿// Copyright (c) 2018 Ark S.r.l. All rights reserved.
// Licensed under the MIT License. See LICENSE file for license information. 
using NodaTime;
using NodaTime.Text;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Ark.Tools.Nodatime
{
    public class InstantConverter : TypeConverter
    {
        private readonly InstantPattern _pattern = InstantPattern.ExtendedIso;
        private static Type[] _supportedFrom = new[]
        {
            typeof(string),typeof(Instant)
        };

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (_supportedFrom.Contains(sourceType)) return true;

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is Instant i)
            {
                return i;
            }
            if (value is string s)
            {
                var r = _pattern.WithCulture(culture).Parse(s);
                if (r.Success)
                    return r.Value;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string) || destinationType == typeof(DateTime))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return _pattern.Format((Instant)value);
            if (destinationType == typeof(DateTime))
                return ((Instant)value).ToDateTimeUtc();

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
