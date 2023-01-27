﻿namespace Aegis.Core.Events.Audit.IdentityProvider
{
	using Aegis.Core.Contracts.Application.Events;
	using Aegis.Core.Events.AuditEvents;
	using Aegis.Enums.AuditEvents;

	/// <summary>
	/// Create User Succeeded Audit Event
	/// </summary>
	/// <seealso cref="IAuditEvent" />
	public sealed record CreateUserSucceededAuditEvent : AuditEvent
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CreateUserSucceededAuditEvent"/> class.
		/// </summary>
		/// <param name="subjectId">The subject identifier.</param>
		/// <param name="summary">The summary.</param>
		/// <param name="oldValues">The old values.</param>
		/// <param name="newValues">The new values.</param>
		public CreateUserSucceededAuditEvent(Guid subjectId, string summary, string? oldValues = null, string? newValues = null)
			: base(true, AuditModule.IdentityProvider, AuditAction.Create, AuditSubject.User, subjectId, summary, false, oldValues, newValues)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CreateUserSucceededAuditEvent"/> class.
		/// </summary>
		/// <param name="subjectId">The subject identifier.</param>
		/// <param name="summary">The summary.</param>
		/// <param name="serviceInitiated">if set to <c>true</c> [service initiated].</param>
		/// <param name="oldValues">The old values.</param>
		/// <param name="newValues">The new values.</param>
		public CreateUserSucceededAuditEvent(Guid subjectId, string summary, bool serviceInitiated, string? oldValues = null, string? newValues = null)
			: base(true, AuditModule.IdentityProvider, AuditAction.Create, AuditSubject.User, subjectId, summary, serviceInitiated, oldValues, newValues)
		{
		}
	}
}