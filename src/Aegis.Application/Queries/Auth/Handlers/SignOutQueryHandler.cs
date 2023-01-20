﻿namespace Aegis.Application.Queries.Auth.Handlers
{
	using Aegis.Application.Constants;
	using Aegis.Application.Contracts;
	using Aegis.Application.Contracts.CQRS;
	using Aegis.Application.Exceptions;
	using Aegis.Models.Auth;
	using Aegis.Models.Shared;

	using Duende.IdentityServer.Models;
	using Duende.IdentityServer.Services;

	using Microsoft.Extensions.Logging;

	/// <summary>
	/// SignOut Query Handler
	/// </summary>
	/// <seealso cref="Aegis.Application.Contracts.CQRS.IQueryHandler&lt;Aegis.Application.Queries.Auth.SignOutQuery, Aegis.Models.Auth.SignOutQueryResult&gt;" />
	public sealed class SignOutQueryHandler : IQueryHandler<SignOutQuery, SignOutQueryResult>
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly ILogger<SignOutQueryHandler> _logger;

		/// <summary>
		/// The interaction.
		/// </summary>
		private readonly IIdentityServerInteractionService _interaction;

		/// <summary>
		/// Initializes a new instance of the <see cref="SignOutQueryHandler"/> class.
		/// </summary>
		/// <param name="logger">The logger.</param>
		/// <param name="interaction">The interaction.</param>
		public SignOutQueryHandler(
			ILogger<SignOutQueryHandler> logger,
			IIdentityServerInteractionService interaction)
		{
			_logger = logger;
			_interaction = interaction;
		}

		/// <summary>
		/// Handles the specified query.
		/// </summary>
		/// <param name="query">The query.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns><see cref="SignOutQueryResult"/></returns>
		/// <exception cref="Aegis.Application.Exceptions.IdentityProviderException"></exception>
		public async Task<SignOutQueryResult> Handle(SignOutQuery query, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Handling {name}", nameof(SignOutQuery));
			SignOutQueryResult signOutQueryResult = SignOutQueryResult.Show(true);

			try
			{
				_logger.LogDebug("SignOutQueryHandler: get logout context.");
				LogoutRequest? context = await _interaction.GetLogoutContextAsync(query.LogoutId);

				if (context is not null)
				{
					signOutQueryResult = SignOutQueryResult.Show(context.ShowSignoutPrompt);
				}
			}
			catch (Exception ex) when (ex is not IAegisException)
			{
				_logger.LogError(ex, "SignOutQueryHandler Error: {Message}", ex.Message);
				throw new IdentityProviderException(IdentityProviderConstants.SomethingWentWrongWithSignOut, ex.Message, ex);
			}

			_logger.LogDebug("Handled {name}", nameof(SignOutQuery));
			return signOutQueryResult;
		}
	}
}
