#region License

// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

#endregion

using System;

namespace Newtonsoft.Json.Utilities
{
	internal enum ParserTimeZone
	{
		Unspecified,
		Utc,
		LocalWestOfUtc,
		LocalEastOfUtc
	}

	internal struct DateTimeParser
	{
		#region Constructors/Destructors

		static DateTimeParser()
		{
			Power10 = new[] { -1, 10, 100, 1000, 10000, 100000, 1000000 };

			Lzyyyy = "yyyy".Length;
			Lzyyyy_ = "yyyy-".Length;
			Lzyyyy_MM = "yyyy-MM".Length;
			Lzyyyy_MM_ = "yyyy-MM-".Length;
			Lzyyyy_MM_dd = "yyyy-MM-dd".Length;
			Lzyyyy_MM_ddT = "yyyy-MM-ddT".Length;
			LzHH = "HH".Length;
			LzHH_ = "HH:".Length;
			LzHH_mm = "HH:mm".Length;
			LzHH_mm_ = "HH:mm:".Length;
			LzHH_mm_ss = "HH:mm:ss".Length;
			Lz_ = "-".Length;
			Lz_zz = "-zz".Length;
			Lz_zz_ = "-zz:".Length;
			Lz_zz_zz = "-zz:zz".Length;
		}

		#endregion

		#region Fields/Constants

		private const short MaxFractionDigits = 7;

		private static readonly int LzHH;
		private static readonly int LzHH_;
		private static readonly int LzHH_mm;
		private static readonly int LzHH_mm_;
		private static readonly int LzHH_mm_ss;
		private static readonly int Lz_;
		private static readonly int Lz_zz;
		private static readonly int Lz_zz_;
		private static readonly int Lz_zz_zz;
		private static readonly int Lzyyyy;
		private static readonly int Lzyyyy_;
		private static readonly int Lzyyyy_MM;
		private static readonly int Lzyyyy_MM_;
		private static readonly int Lzyyyy_MM_dd;
		private static readonly int Lzyyyy_MM_ddT;
		private static readonly int[] Power10;
		public int Day;
		public int Fraction;
		public int Hour;
		public int Minute;
		public int Month;
		public int Second;
		public int Year;
		public ParserTimeZone Zone;
		public int ZoneHour;
		public int ZoneMinute;
		private int _length;
		private string _text;

		#endregion

		#region Methods/Operators

		public bool Parse(string text)
		{
			this._text = text;
			this._length = text.Length;

			if (this.ParseDate(0) && this.ParseChar(Lzyyyy_MM_dd, 'T') && this.ParseTimeAndZoneAndWhitespace(Lzyyyy_MM_ddT))
				return true;

			return false;
		}

		private bool Parse2Digit(int start, out int num)
		{
			if (start + 1 < this._length)
			{
				int digit1 = this._text[start] - '0';
				int digit2 = this._text[start + 1] - '0';
				if (0 <= digit1 && digit1 < 10
					&& 0 <= digit2 && digit2 < 10)
				{
					num = (digit1 * 10) + digit2;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool Parse4Digit(int start, out int num)
		{
			if (start + 3 < this._length)
			{
				int digit1 = this._text[start] - '0';
				int digit2 = this._text[start + 1] - '0';
				int digit3 = this._text[start + 2] - '0';
				int digit4 = this._text[start + 3] - '0';
				if (0 <= digit1 && digit1 < 10
					&& 0 <= digit2 && digit2 < 10
					&& 0 <= digit3 && digit3 < 10
					&& 0 <= digit4 && digit4 < 10)
				{
					num = (((((digit1 * 10) + digit2) * 10) + digit3) * 10) + digit4;
					return true;
				}
			}
			num = 0;
			return false;
		}

		private bool ParseChar(int start, char ch)
		{
			return (start < this._length && this._text[start] == ch);
		}

		private bool ParseDate(int start)
		{
			return (this.Parse4Digit(start, out this.Year)
					&& 1 <= this.Year
					&& this.ParseChar(start + Lzyyyy, '-')
					&& this.Parse2Digit(start + Lzyyyy_, out this.Month)
					&& 1 <= this.Month
					&& this.Month <= 12
					&& this.ParseChar(start + Lzyyyy_MM, '-')
					&& this.Parse2Digit(start + Lzyyyy_MM_, out this.Day)
					&& 1 <= this.Day
					&& this.Day <= DateTime.DaysInMonth(this.Year, this.Month));
		}

		private bool ParseTime(ref int start)
		{
			if (!(this.Parse2Digit(start, out this.Hour)
				&& this.Hour < 24
				&& this.ParseChar(start + LzHH, ':')
				&& this.Parse2Digit(start + LzHH_, out this.Minute)
				&& this.Minute < 60
				&& this.ParseChar(start + LzHH_mm, ':')
				&& this.Parse2Digit(start + LzHH_mm_, out this.Second)
				&& this.Second < 60))
				return false;

			start += LzHH_mm_ss;
			if (this.ParseChar(start, '.'))
			{
				this.Fraction = 0;
				int numberOfDigits = 0;

				while (++start < this._length && numberOfDigits < MaxFractionDigits)
				{
					int digit = this._text[start] - '0';
					if (digit < 0 || digit > 9)
						break;

					this.Fraction = (this.Fraction * 10) + digit;

					numberOfDigits++;
				}

				if (numberOfDigits < MaxFractionDigits)
				{
					if (numberOfDigits == 0)
						return false;

					this.Fraction *= Power10[MaxFractionDigits - numberOfDigits];
				}
			}
			return true;
		}

		private bool ParseTimeAndZoneAndWhitespace(int start)
		{
			return (this.ParseTime(ref start) && this.ParseZone(start));
		}

		private bool ParseZone(int start)
		{
			if (start < this._length)
			{
				char ch = this._text[start];
				if (ch == 'Z' || ch == 'z')
				{
					this.Zone = ParserTimeZone.Utc;
					start++;
				}
				else
				{
					if (start + 5 < this._length
						&& this.Parse2Digit(start + Lz_, out this.ZoneHour)
						&& this.ZoneHour <= 99
						&& this.ParseChar(start + Lz_zz, ':')
						&& this.Parse2Digit(start + Lz_zz_, out this.ZoneMinute)
						&& this.ZoneMinute <= 99)
					{
						switch (ch)
						{
							case '-':
								this.Zone = ParserTimeZone.LocalWestOfUtc;
								start += Lz_zz_zz;
								break;

							case '+':
								this.Zone = ParserTimeZone.LocalEastOfUtc;
								start += Lz_zz_zz;
								break;
						}
					}
				}
			}

			return (start == this._length);
		}

		#endregion
	}
}