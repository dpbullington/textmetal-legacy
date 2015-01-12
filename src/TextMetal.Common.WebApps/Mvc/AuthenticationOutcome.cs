namespace TextMetal.Common.WebApps.Mvc
{
	public enum AuthenticationOutcome
	{
		/// <summary>
		/// We do not care who you are: access is denied.
		/// </summary>
		None = 0,

		/// <summary>
		/// We know nothing about who you are but that is OK.
		/// </summary>
		Anonymous = 1,

		/// <summary>
		/// We do not know you are who you say you are but that is OK.
		/// You presented a barer instrument (URL, code, etc) and thats passed the sniff test.
		/// This should be for idempotent actions only.
		/// </summary>
		Evidenced = 2,

		/// <summary>
		/// We know you are who you say you are.
		/// We do not know if you are authorized however.
		/// </summary>
		Authenticated = 3,
	}
}