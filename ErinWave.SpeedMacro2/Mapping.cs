using System.Text.RegularExpressions;

namespace ErinWave.SpeedMacro
{
	public partial class Mapping
	{
		public static (int x, int y) ParseMouse(string str)
		{
			var match = MouseRegex().Match(str);
			if (match.Success)
			{
				int x = int.Parse(match.Groups[1].Value);
				int y = int.Parse(match.Groups[2].Value);
				return (x, y);
			}
			throw new FormatException("Parse Error");
		}

		public static string ParseKey(string str)
		{
			var match = KeyRegex().Match(str);
			if (match.Success)
			{
				return match.Groups[1].Value;
			}
			throw new FormatException("Parse Error");
		}

		public static int ParseWait(string str)
		{
			var match = WaitRegex().Match(str);
			if (match.Success)
			{
				return int.Parse(match.Groups[1].Value);
			}
			throw new FormatException("Parse Error");
		}

		public static string KeysString(string keyString)
		{
			string toString = string.Empty;
			switch (keyString)
			{
				case "Capital": toString = "CapsLock"; break;
				case "D0": toString = "0"; break;
				case "D1": toString = "1"; break;
				case "D2": toString = "2"; break;
				case "D3": toString = "3"; break;
				case "D4": toString = "4"; break;
				case "D5": toString = "5"; break;
				case "D6": toString = "6"; break;
				case "D7": toString = "7"; break;
				case "D8": toString = "8"; break;
				case "D9": toString = "9"; break;
				case "Add": toString = "+"; break;
				case "Subtract": toString = "-"; break;
				case "Multiply": toString = "*"; break;
				case "Divide": toString = "/"; break;
				case "Up": toString = "↑"; break;
				case "Down": toString = "↓"; break;
				case "Left": toString = "←"; break;
				case "Right": toString = "→"; break;
				case "Escape": toString = "ESC"; break;
				case "Oemtilde": toString = "`"; break;
				case "OemMinus": toString = "-"; break;
				case "Oemplus": toString = "="; break;
				case "Oem5": toString = "\\"; break;
				case "Next": toString = "PageDown"; break;
				case "OemOpenBrackets": toString = "["; break;
				case "Oem6": toString = "]"; break;
				case "Oem1": toString = ";"; break;
				case "Oem7": toString = "'"; break;
				case "Oemcomma": toString = ","; break;
				case "OemPeriod": toString = "."; break;
				case "OemQuestion": toString = "/"; break;
				case "Return": toString = "Enter"; break;
				case "Scroll": toString = "ScrollLock"; break;
				case "LWin": toString = "Win"; break;
				case "HanjaMode": toString = "Hanja"; break;
				case "KanaMode": toString = "Kana"; break;
				case "Decimal": toString = "."; break;
				case "NumPad0": toString = "N0"; break;
				case "NumPad1": toString = "N1"; break;
				case "NumPad2": toString = "N2"; break;
				case "NumPad3": toString = "N3"; break;
				case "NumPad4": toString = "N4"; break;
				case "NumPad5": toString = "N5"; break;
				case "NumPad6": toString = "N6"; break;
				case "NumPad7": toString = "N7"; break;
				case "NumPad8": toString = "N8"; break;
				case "NumPad9": toString = "N9"; break;
				default: toString = keyString; break;
			}
			return toString;
		}

		public static string SendKeysString(string keyString)
		{
			string[] key = keyString.Split('+');
			string toString = string.Empty;
			for (int i = 0; i < key.Length; i++)
			{
				switch (key[i])
				{
					case "Ctrl": toString += "^"; break;
					case "Alt": toString += "%"; break;
					case "Shift": toString += "+"; break;
					case "Back": toString += "{BACKSPACE}"; break;
					case "CapsLock": toString += "{CAPSLOCK}"; break;
					case "Delete": toString += "{DELETE}"; break;
					case "↓": toString += "{DOWN}"; break;
					case "End": toString += "{END}"; break;
					case "Enter": toString += "{ENTER}"; break;
					case "ESC": toString += "{ESC}"; break;
					case "Home": toString += "{HOME}"; break;
					case "Insert": toString += "{INSERT}"; break;
					case "←": toString += "{LEFT}"; break;
					case "NumLock": toString += "{NUMLOCK}"; break;
					case "PageDown": toString += "{PGDN}"; break;
					case "PageUp": toString += "{PGUP}"; break;
					case "PrintScreen": toString += "{PRTSC}"; break;
					case "→": toString += "{RIGHT}"; break;
					case "ScrollLock": toString += "{SCROLLLOCK}"; break;
					case "Tab": toString += "{TAB}"; break;
					case "↑": toString += "{UP}"; break;
					case "F1": toString += "{F1}"; break;
					case "F2": toString += "{F2}"; break;
					case "F3": toString += "{F3}"; break;
					case "F4": toString += "{F4}"; break;
					case "F5": toString += "{F5}"; break;
					case "F6": toString += "{F6}"; break;
					case "F7": toString += "{F7}"; break;
					case "F8": toString += "{F8}"; break;
					case "F9": toString += "{F9}"; break;
					case "F10": toString += "{F10}"; break;
					case "F11": toString += "{F11}"; break;
					case "F12": toString += "{F12}"; break;
					case "+": toString += "{ADD}"; break;
					case "-": toString += "{SUBTRACT}"; break;
					case "*": toString += "{MULTIPLY}"; break;
					case "/": toString += "{DIVIDE}"; break;
					default:
						if (key[i].Length == 1 && key[i][0] >= 'A' && key[i][0] <= 'Z')
							toString += key[i].ToLower();
						else
							toString += key[i]; break;
				}
			}
			return toString;
		}

		[GeneratedRegex(@"\((\d+),\s*(\d+)\)")]
		public static partial Regex MouseRegex();
		[GeneratedRegex(@"\(([^)]+)\)")]
		public static partial Regex KeyRegex();
		[GeneratedRegex(@"\((\d+)ms\)")]
		public static partial Regex WaitRegex();
	}
}
