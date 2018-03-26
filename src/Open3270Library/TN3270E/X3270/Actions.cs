#region License
/* 
 *
 * Open3270 - A C# implementation of the TN3270/TN3270E protocol
 *
 *   Copyright © 2004-2006 Michael Warriner. All rights reserved
 * 
 * This is free software; you can redistribute it and/or modify it
 * under the terms of the GNU Lesser General Public License as
 * published by the Free Software Foundation; either version 2.1 of
 * the License, or (at your option) any later version.
 *
 * This software is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this software; if not, write to the Free
 * Software Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA
 * 02110-1301 USA, or see the FSF site: http://www.fsf.org.
 */
#endregion
using System;
using System.Collections.Generic;

namespace Open3270.TN3270
{
	internal delegate bool ActionDelegate(params object[] args);

	internal class Actions
	{
		private readonly Dictionary<string, XtActionRec> _actions;
		private readonly Telnet _telnet;
		private List<byte[]> datacapture = null;
		private List<string> datastringcapture = null;

		internal Actions(Telnet tn)
		{
			_telnet = tn;
			_actions = new Dictionary<string, XtActionRec> {
				{"printtext",     new XtActionRec( false,    new ActionDelegate(_telnet.Print.PrintTextAction )) },
				{"flip",          new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FlipAction )) },
				{"ascii",         new XtActionRec( false,    new ActionDelegate(_telnet.Controller.AsciiAction )) },
				{"dumpxml",       new XtActionRec( false,    new ActionDelegate(_telnet.Controller.DumpXMLAction )) },
				{"asciifield",    new XtActionRec( false,    new ActionDelegate(_telnet.Controller.AsciiFieldAction )) },
				{"attn",          new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.AttnAction )) },
				{"backspace",     new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.BackSpaceAction )) },
				{"backtab",       new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.BackTab_action )) },
				{"circumnot",     new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.CircumNotAction )) },
				{"clear",         new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.ClearAction )) },
				{"cursorselect",  new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.CursorSelectAction )) },
				{"delete",        new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.DeleteAction )) },
				{"deletefield",   new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.DeleteFieldAction )) },
				{"deleteword",    new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.DeleteWordAction )) },
				{"down",          new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorDown )) },
				{"dup",           new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.DupAction )) },
				{"emulateinput",  new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.EmulateInputAction )) },
				{"enter",         new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.EnterAction )) },
				{"erase",         new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.EraseAction )) },
				{"eraseeof",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.EraseEndOfFieldAction )) },
				{"eraseinput",    new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.EraseInputAction)) },
				{"fieldend",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldEndAction )) },
				{"fields",        new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldsAction )) },
				{"fieldget",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldGetAction )) },
				{"fieldset",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldSetAction )) },
				{"fieldmark",     new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldMarkAction )) },
				{"fieldexit",     new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.FieldExitAction )) },
				{"hexString",     new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.HexStringAction )) },
				{"home",          new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.HomeAction )) },
				{"insert",        new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.InsertAction )) },
				{"interrupt",     new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.InterruptAction )) },
				{"key",           new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.SendKeyAction )) },
				{"left",          new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.LeftAction )) },
				{"left2",         new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorLeft2Positions )) },
				{"monocase",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MonoCaseAction )) },
				{"movecursor",    new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorAction )) },
				{"Newline",       new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorToNewLine)) },
				{"NextWord",      new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorToNextUnprotectedWord )) },
				{"PA",            new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.PAAction )) },
				{"PF",            new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.PFAction )) },
				{"PreviousWord",  new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.PreviousWordAction )) },
				{"Reset",         new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.ResetAction )) },
				{"Right",         new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveRight )) },
				{"Right2",        new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorRight2Positions )) },
				{"String",        new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.SendStringAction )) },
				{"SysReq",        new XtActionRec( true,     new ActionDelegate(_telnet.Keyboard.SystemRequestAction )) },
				{"Tab",           new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.TabForwardAction )) },
				{"ToggleInsert",  new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.ToggleInsertAction )) },
				{"ToggleReverse", new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.ToggleReverseAction )) },
				{"Up",            new XtActionRec( false,    new ActionDelegate(_telnet.Keyboard.MoveCursorUp )) }

			};
		}

		//public iaction ia_cause;

		public string[] ia_name = new string[] {
												   "String", "Paste", "Screen redraw", "Keypad", "Default", "Key",
												   "Macro", "Script", "Peek", "Typeahead", "File transfer", "Command",
												   "Keymap"
											   };

		/*
		 * Return a name for an action.
		 */
		string action_name(ActionDelegate action)
		{
			foreach (KeyValuePair<string, XtActionRec> xtAct in _actions)
			{
				if (xtAct.Value.Proc == action)
				{
					return xtAct.Key;
				}
			}
			return "(unknown)";
		}


		/*
		 * Wrapper for calling an action internally.
		 */
		public bool action_internal(ActionDelegate action, params object[] args)
		{
			return action(args);
		}
		
		public void action_output(string data)
		{
			action_output(data, false);
		}
		private string encodeXML(string data)
		{
			//data = data.Replace("\"", "&quot;");
			//data = data.Replace(">", "&gt;");
			data = data.Replace("<", "&lt;");
			data = data.Replace("&", "&amp;");
			return data;
		}
		public void action_output(string data, bool encode)
		{
			if (datacapture == null)
				datacapture = new List<byte[]>();
			if (datastringcapture == null)
				datastringcapture = new List<string>();

			datacapture.Add(System.Text.Encoding.ASCII.GetBytes(data));
			//
			if (encode)
			{
				data = encodeXML(data);
			}
			//
			datastringcapture.Add(data);
		}
		public void action_output(byte[] data, int length)
		{
			action_output(data, length, false);
		}
		public void action_output(byte[] data, int length, bool encode)
		{
			if (datacapture == null)
				datacapture = new List<byte[]>();
			if (datastringcapture == null)
				datastringcapture = new List<string>();

			//
			byte[] temp = new byte[length];
			int i;
			for (i = 0; i < length; i++)
			{
				temp[i] = data[i];
			}
			datacapture.Add(temp);
			string strdata = System.Text.Encoding.ASCII.GetString(temp);
			if (encode)
			{
				strdata = encodeXML(strdata);
			}

			datastringcapture.Add(strdata);
		}
		public string GetStringData(int index)
		{
			if (datastringcapture == null)
				return null;
			if (index >= 0 && index < datastringcapture.Count)
				return datastringcapture[index];
			else
				return null;
		}
		public byte[] GetByteData(int index)
		{
			if (datacapture == null)
				return null;
			if (index >= 0 && index < datacapture.Count)
				return datacapture[index];
			else
				return null;
		}
		public bool KeyboardCommandCausesSubmit(string name)
		{
			if (null == name || !_actions.ContainsKey(name))
			{
				throw new ApplicationException("Sorry, action '" + name + "' is not known");
			}
			return _actions[name].CausesSubmit;
		}
		public bool Execute(bool submit, string name, params object[] args)
		{
			_telnet.Events.Clear();
			// Check that we're connected
			if (!_telnet.IsConnected)
			{
				throw new Open3270.TNHostException("TN3270 Host is not connected", _telnet.DisconnectReason, null);
			}

			datacapture = null;
			datastringcapture = null;

			if (null == name || !_actions.ContainsKey(name))
			{
				throw new ApplicationException("Sorry, action '" + name + "' is not known");
			}
			return _actions[name].Proc(args);
		}



		#region Nested classes

		private class XtActionRec
		{
			public ActionDelegate Proc { get; }
			public bool CausesSubmit { get; }

			public XtActionRec(bool causesSubmit, ActionDelegate fn)
			{
				CausesSubmit = causesSubmit;
				Proc = fn;
			}
		}

		#endregion Nested classes


	}
}
