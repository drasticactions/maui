// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;

namespace Microsoft.Maui.Controls.Xaml.Diagnostics
{
	public class BindingBaseErrorEventArgs : EventArgs
	{
		internal BindingBaseErrorEventArgs(SourceInfo xamlSourceInfo, BindingBase binding, string errorCode, string message, object[] messageArgs)
		{
			XamlSourceInfo = xamlSourceInfo;
			Binding = binding;
			ErrorCode = errorCode;
			Message = message;
			MessageArgs = messageArgs;
		}

		public SourceInfo XamlSourceInfo { get; }
		public BindingBase Binding { get; }
		public string ErrorCode { get; }
		public string Message { get; }
		public object[] MessageArgs { get; }
	}

	public class BindingErrorEventArgs : BindingBaseErrorEventArgs
	{
		internal BindingErrorEventArgs(
			SourceInfo xamlSourceInfo,
			BindingBase binding,
			object bindingsource,
			BindableObject target,
			BindableProperty property,
			string errorCode,
			string message,
			object[] messageArgs) : base(xamlSourceInfo, binding, errorCode, message, messageArgs)
		{
			Source = bindingsource;
			Target = target;
			TargetProperty = property;
		}

		public object Source { get; }
		public BindableObject Target { get; }
		public BindableProperty TargetProperty { get; }
	}
}
