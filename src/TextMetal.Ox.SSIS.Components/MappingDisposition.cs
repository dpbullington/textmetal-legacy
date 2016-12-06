/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

namespace TextMetal.Ox.SSIS.Components
{
	public enum MappingDisposition : sbyte
	{
		UnknownToPackageConfig = -1,
		NotMappedInPackageConfig = 0,
		IsMappedInPackageConfig = 1,
	}
}