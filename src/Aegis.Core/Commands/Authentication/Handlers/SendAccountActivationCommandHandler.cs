﻿#region copyright
//----------------------------------------------------------------------
// Copyright 2023 MNB Software
// Licensed under the Apache License, Version 2.0
// You may obtain a copy at http://www.apache.org/licenses/LICENSE-2.0
//----------------------------------------------------------------------
#endregion

namespace Aegis.Core.Commands.Authentication.Handlers
{
	using Aegis.Core.Constants;
	using Aegis.Core.Constants.Services;
	using Aegis.Core.Contracts;
	using Aegis.Core.Contracts.CQRS;
	using Aegis.Core.Events.AuditEvents.IdentityProvider;
	using Aegis.Core.Exceptions;
	using Aegis.Core.Helpers;
	using Aegis.Models.Settings;
	using Aegis.Models.Shared;
	using Aegis.Persistence.Entities.IdentityProvider;

	using MediatR;

	using Microsoft.AspNetCore.DataProtection;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;

	using Microsoft.Extensions.Logging;

	using Newtonsoft.Json;

	/// <summary>
	/// Send Account Activation Command Handler
	/// </summary>
	/// <seealso cref="Aegis.Core.Contracts.CQRS.ICommandHandler&lt;Aegis.Core.Commands.Authentication.SendAccountActivationCommand, Aegis.Models.Shared.HandlerResult&gt;" />
	public sealed class SendAccountActivationCommandHandler : ICommandHandler<SendAccountActivationCommand, HandlerResult>
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<SendAccountActivationCommandHandler> _logger;

		/// <summary>
		/// The mediator
		/// </summary>
		private readonly IMediator _mediator;

		/// <summary>
		/// The data protector
		/// </summary>
		private readonly IDataProtector _dataProtector;

		/// <summary>
		/// The mail sender service
		/// </summary>
		private readonly IMailSenderService _mailSenderService;

		/// <summary>
		/// The application settings
		/// </summary>
		private readonly AppSettings _appSettings;

		/// <summary>
		/// The user manager
		/// </summary>
		private readonly UserManager<AegisUser> _userManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="SendAccountActivationCommandHandler" /> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="dataProtectionProvider">The data protection provider.</param>
		/// <param name="mailSenderService">The mail sender service.</param>
		/// <param name="appSettings">The application settings.</param>
		/// <param name="userManager">The user manager.</param>
		public SendAccountActivationCommandHandler(
			ILogger<SendAccountActivationCommandHandler> logger,
			IDataProtectionProvider dataProtectionProvider,
			IMediator mediator,
			IMailSenderService mailSenderService,
			AppSettings appSettings,
			UserManager<AegisUser> userManager)
		{
			_logger = logger;
			_mediator = mediator;
			_dataProtector = dataProtectionProvider.CreateProtector(ProtectorHelpers.QueryStringProtector);
			_mailSenderService = mailSenderService;
			_appSettings = appSettings;
			_userManager = userManager;
		}

		/// <summary>
		/// Handles the specified command.
		/// </summary>
		/// <param name="command">The command.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>
		///  <see cref="andlerResult" />
		/// </returns>
		/// <exception cref="IdentityProviderException"></exception>
		public async Task<HandlerResult> Handle(SendAccountActivationCommand command, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Handling {name}", nameof(SendAccountActivationCommand));
			HandlerResult handlerResult = HandlerResult.Succeeded();

			try
			{
				_logger.LogDebug("SendAccountActivationCommandHandler: check if user exists.");
				AegisUser? user = await _userManager.FindByIdAsync(command.UserId!);

				if (user is not null)
				{
					string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
					ActivateAccountCommand activateAccountCommand = new ActivateAccountCommand
					{
						UserId = user.Id.ToString(),
						Token = token
					};

					QueryString res = ProtectorHelpers.ProtectQueryString(_dataProtector, activateAccountCommand);
					string link = $"https://{_appSettings.PublicDomain}/ActivateAccount{res}";
					await _mailSenderService.SendEmailConfirmationLinkAsync(link, user.Email!);
					await _mediator.Publish(new SendActivateAccountSucceededAuditEvent(user.Id, "Send Activate Account email"), cancellationToken);
				}
			}
			catch (Exception ex) when (ex is not IAegisException)
			{
				_logger.LogError(ex, "SendAccountActivationCommandHandler Error: {Message}", ex.Message);
				throw new IdentityProviderException(IdentityProviderConstants.SomethingWentWrong, ex.Message, ex);
			}

			_logger.LogDebug("Handled {name}", nameof(SendAccountActivationCommand));
			return handlerResult;
		}
	}
}
