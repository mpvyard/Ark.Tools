﻿// Copyright (c) 2018 Ark S.r.l. All rights reserved.
// Licensed under the MIT License. See LICENSE file for license information. 
using EnsureThat;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace Ark.Tools.Solid
{
    public class CommandFluentValidateDecorator<TCommand>
        : ICommandHandler<TCommand> 
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IValidator<TCommand> _validator;

        public CommandFluentValidateDecorator(ICommandHandler<TCommand> decorated, IValidator<TCommand> validator)
        {
            Ensure.Any.IsNotNull(decorated, nameof(decorated));
            Ensure.Any.IsNotNull(validator, nameof(validator));

            _decorated = decorated;
            _validator = validator;
        }

        public void Execute(TCommand query)
        {
            _validator.ValidateAndThrow(query);
            _decorated.Execute(query);
        }

        public async Task ExecuteAsync(TCommand query, CancellationToken ctk = default(CancellationToken))
        {
            await _validator.ValidateAndThrowAsync(query);
            await _decorated.ExecuteAsync(query, ctk);
        }
    }
}
