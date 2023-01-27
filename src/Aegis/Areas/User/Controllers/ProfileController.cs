﻿#region copyright
//----------------------------------------------------------------------
// Copyright 2023 MNB Software
// Licensed under the Apache License, Version 2.0
// You may obtain a copy at http://www.apache.org/licenses/LICENSE-2.0
//----------------------------------------------------------------------
#endregion

namespace Aegis.Areas.User.Controllers
{
	using Aegis.Core.Constants;

	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	[Authorize]
	[Area(ApplicationConstants.UserArea)]
	public class ProfileController : Controller
	{
		[HttpGet]
		public IActionResult Index()
			=> this.View();
	}
}
