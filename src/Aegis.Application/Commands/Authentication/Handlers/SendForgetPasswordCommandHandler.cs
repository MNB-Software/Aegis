﻿namespace Aegis.Application.Commands.Authentication.Handlers
{
	using Aegis.Application.Constants;
	using Aegis.Application.Constants.Services;
	using Aegis.Application.Contracts;
	using Aegis.Application.Contracts.CQRS;
	using Aegis.Application.Exceptions;
	using Aegis.Models.Settings;
	using Aegis.Models.Shared;
	using Aegis.Persistence.Entities.IdentityProvider;

	using Microsoft.AspNetCore.DataProtection;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;

	using Microsoft.Extensions.Logging;

	/// <summary>
	/// Send Forget Password CommandHandler
	/// </summary>
	/// <seealso cref="Aegis.Application.Contracts.CQRS.ICommandHandler&lt;Aegis.Application.Commands.Authentication.SendForgetPasswordCommand, Aegis.Models.Shared.HandlerResult&gt;" />
	public sealed class SendForgetPasswordCommandHandler : ICommandHandler<SendForgetPasswordCommand, HandlerResult>
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<SendForgetPasswordCommandHandler> _logger;

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
		/// Initializes a new instance of the <see cref="SendForgetPasswordCommandHandler" /> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="mailSenderService">The mail sender service.</param>
		/// <param name="appSettings">The application settings.</param>
		/// <param name="userManager">The user manager.</param>
		public SendForgetPasswordCommandHandler(
			ILogger<SendForgetPasswordCommandHandler> logger,
			IMailSenderService mailSenderService,
			AppSettings appSettings,
			UserManager<AegisUser> userManager)
		{
			_logger = logger;
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
		///   <see cref="andlerResult" />
		/// </returns>
		/// <exception cref="IdentityProviderException"></exception>
		public async Task<HandlerResult> Handle(SendForgetPasswordCommand command, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Handling {name}", nameof(SendForgetPasswordCommand));
			HandlerResult handlerResult = HandlerResult.Succeeded();

			try
			{
				_logger.LogDebug("SendForgetPasswordCommandHandler: check if user exists.");
				AegisUser? user = await _userManager.FindByEmailAsync(command.Email!);

				if (user is not null)
				{
					string token = await _userManager.GeneratePasswordResetTokenAsync(user);

					QueryString res = QueryString.Create(new List<KeyValuePair<string, string?>>
					{
						new KeyValuePair<string, string?>("UserId", user.Id.ToString()),
						new KeyValuePair<string, string?>("Token", token)
					});

					string link = $"https://{_appSettings.PublicDomain}/ResetPassword{res}";
					await _mailSenderService.SendResetPasswordLinkAsync(link, user.Email!);
				}
			}
			catch (Exception ex) when (ex is not IAegisException)
			{
				_logger.LogError(ex, "SendForgetPasswordCommandHandler Error: {Message}", ex.Message);
				throw new IdentityProviderException(IdentityProviderConstants.SomethingWentWrong, ex.Message, ex);
			}

			_logger.LogDebug("Handled {name}", nameof(SendForgetPasswordCommand));
			return handlerResult;
		}
	}
}
