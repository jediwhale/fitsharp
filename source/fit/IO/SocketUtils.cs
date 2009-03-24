// FitNesse.NET
// Copyright © 2007,2008 Syterra Software Inc. Includes work by Object Mentor, Inc., (c) 2002 Cunningham & Cunningham, Inc.
// This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License version 2.
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more details.

using System.Net.Sockets;
using System.Text;

namespace fitnesse.fitserver
{
	public class SocketUtils
	{
		private	const SocketFlags flags = new SocketFlags();

		public static string ReceiveStringOfLength(ISocketWrapper socketWrapper, int byteCountToReceive)
		{
			byte[] bytes = new byte[byteCountToReceive];
			char[] characters = new char[byteCountToReceive];
			int byteCountReceived = 0;
			while (byteCountReceived < byteCountToReceive)
			{
			    byteCountReceived += socketWrapper.Receive(bytes, byteCountReceived, byteCountToReceive - byteCountReceived, flags);
			}
			int charCount = Encoding.UTF8.GetDecoder().GetChars(bytes, 0, byteCountToReceive, characters, 0);
			return new StringBuilder(charCount).Append(characters, 0, charCount).ToString();
		}
	}
}
