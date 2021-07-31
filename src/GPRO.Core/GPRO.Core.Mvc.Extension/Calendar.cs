using System;
namespace GPRO.Core.Mvc.Extension
{
	public class Calendar
	{
		private double timeZone = 7.0;
		public long getJulius(int intNgay, int intThang, int intNam)
		{
			int num = (14 - intThang) / 12;
			int num2 = intNam + 4800 - num;
			int num3 = intThang + 12 * num - 3;
			int num4 = intNgay + (153 * num3 + 2) / 5 + 365 * num2 + num2 / 4 - num2 / 100 + num2 / 400 - 32045;
			if (num4 < 2299161)
			{
				num4 = intNgay + (153 * num3 + 2) / 5 + 365 * num2 + num2 / 4 - 32083;
			}
			return (long)num4;
		}
		public string jdTosDate(int jd)
		{
			int num2;
			int num3;
			if (jd > 2299160)
			{
				int num = jd + 32044;
				num2 = (4 * num + 3) / 146097;
				num3 = num - num2 * 146097 / 4;
			}
			else
			{
				num2 = 0;
				num3 = jd + 32082;
			}
			int num4 = (4 * num3 + 3) / 1461;
			int num5 = num3 - 1461 * num4 / 4;
			int num6 = (5 * num5 + 2) / 153;
			int num7 = num5 - (153 * num6 + 2) / 5 + 1;
			int num8 = num6 + 3 - 12 * (num6 / 10);
			int num9 = num2 * 100 + num4 - 4800 + num6 / 10;
			return string.Concat(new string[]
			{
				num7.ToString(),
				"/",
				num8.ToString(),
				"/",
				num9.ToString()
			});
		}
		public int getNewMoonDay(int k)
		{
			double num = (double)k / 1236.85;
			double num2 = num * num;
			double num3 = num2 * num;
			double num4 = 0.017453292519943295;
			double num5 = 2415020.75933 + 29.53058868 * (double)k + 0.0001178 * num2 - 1.55E-07 * num3;
			num5 += 0.00033 * Math.Sin((166.56 + 132.87 * num - 0.009173 * num2) * num4);
			double num6 = 359.2242 + 29.10535608 * (double)k - 3.33E-05 * num2 - 3.47E-06 * num3;
			double num7 = 306.0253 + 385.81691806 * (double)k + 0.0107306 * num2 + 1.236E-05 * num3;
			double num8 = 21.2964 + 390.67050646 * (double)k - 0.0016528 * num2 - 2.39E-06 * num3;
			double num9 = (0.1734 - 0.000393 * num) * Math.Sin(num6 * num4) + 0.0021 * Math.Sin(2.0 * num4 * num6);
			num9 = num9 - 0.4068 * Math.Sin(num7 * num4) + 0.0161 * Math.Sin(num4 * 2.0 * num7);
			num9 -= 0.0004 * Math.Sin(num4 * 3.0 * num7);
			num9 = num9 + 0.0104 * Math.Sin(num4 * 2.0 * num8) - 0.0051 * Math.Sin(num4 * (num6 + num7));
			num9 = num9 - 0.0074 * Math.Sin(num4 * (num6 - num7)) + 0.0004 * Math.Sin(num4 * (2.0 * num8 + num6));
			num9 = num9 - 0.0004 * Math.Sin(num4 * (2.0 * num8 - num6)) - 0.0006 * Math.Sin(num4 * (2.0 * num8 + num7));
			num9 = num9 + 0.001 * Math.Sin(num4 * (2.0 * num8 - num7)) + 0.0005 * Math.Sin(num4 * (2.0 * num7 + num6));
			double num10;
			if (num < -11.0)
			{
				num10 = 0.001 + 0.000839 * num + 0.0002261 * num2 - 8.45E-06 * num3 - 8.1E-08 * num * num3;
			}
			else
			{
				num10 = -0.000278 + 0.000265 * num + 0.000262 * num2;
			}
			double num11 = num5 + num9 - num10;
			return (int)(num11 + 0.5 + this.timeZone / 24.0);
		}
		public int getSunLongitude(int jdn)
		{
			double num = ((double)jdn - 2451545.5 - this.timeZone / 24.0) / 36525.0;
			double num2 = num * num;
			double num3 = 0.017453292519943295;
			double num4 = 357.5291 + 35999.0503 * num - 0.0001559 * num2 - 4.8E-07 * num * num2;
			double num5 = 280.46645 + 36000.76983 * num + 0.0003032 * num2;
			double num6 = (1.9146 - 0.004817 * num - 1.4E-05 * num2) * Math.Sin(num3 * num4);
			num6 = num6 + (0.019993 - 0.000101 * num) * Math.Sin(num3 * 2.0 * num4) + 0.00029 * Math.Sin(num3 * 3.0 * num4);
			double num7 = num5 + num6;
			num7 *= num3;
			num7 -= 6.2831853071795862 * (double)((int)(num7 / 6.2831853071795862));
			return (int)(num7 / 3.1415926535897931 * 6.0);
		}
		public int getLunarMonthll(int intNam)
		{
			double num = (double)(this.getJulius(31, 12, intNam) - 2415021L);
			double num2 = (double)((int)(num / 29.530588853));
			double num3 = (double)this.getNewMoonDay((int)num2);
			double num4 = (double)this.getSunLongitude((int)num3);
			if (num4 >= 9.0)
			{
				num3 = (double)this.getNewMoonDay((int)num2 - 1);
			}
			return (int)num3;
		}
		public int getLeapMonthOffset(double a11)
		{
			int num = (int)((a11 - 2415021.0769986948) / 29.530588853 + 0.5);
			int num2 = 1;
			double num3 = (double)this.getSunLongitude(this.getNewMoonDay(num + num2));
			double num4;
			do
			{
				num4 = num3;
				num2++;
				num3 = (double)this.getSunLongitude(this.getNewMoonDay(num + num2));
			}
			while (num3 != num4 && num2 < 14);
			return num2 - 1;
		}
		public string convertSolar2Lunar(int intNgay, int intThang, int intNam)
		{
			double num = (double)this.getJulius(intNgay, intThang, intNam);
			int num2 = (int)((num - 2415021.0769986948) / 29.530588853);
			double num3 = (double)this.getNewMoonDay(num2 + 1);
			if (num3 > num)
			{
				num3 = (double)this.getNewMoonDay(num2);
			}
			double num4 = (double)this.getLunarMonthll(intNam);
			double num5 = num4;
			double num6;
			if (num4 >= num3)
			{
				num6 = (double)intNam;
				num4 = (double)this.getLunarMonthll(intNam - 1);
			}
			else
			{
				num6 = (double)(intNam + 1);
				num5 = (double)this.getLunarMonthll(intNam + 1);
			}
			double num7 = num - num3 + 1.0;
			int num8 = (int)((num3 - num4) / 29.0);
			double num9 = (double)(num8 + 11);
			if (num5 - num4 > 365.0)
			{
				int leapMonthOffset = this.getLeapMonthOffset(num4);
				if (num8 >= leapMonthOffset)
				{
					num9 = (double)(num8 + 10);
					if (num8 == leapMonthOffset)
					{
					}
				}
			}
			if (num9 > 12.0)
			{
				num9 -= 12.0;
			}
			if (num9 >= 11.0 && num8 < 4)
			{
				num6 -= 1.0;
			}
			string str = num7.ToString();
			string str2 = num9.ToString();
			string text = num6.ToString();
			return str + "/" + str2;
		}
	}
}
