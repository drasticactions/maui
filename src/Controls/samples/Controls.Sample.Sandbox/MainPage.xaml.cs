using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Maui.Controls.Sample
{
	public partial class MainPage : ContentPage
	{
		public ObservableCollection<StopTimesGroup> StopTimesGroups { get; set; } = new ObservableCollection<StopTimesGroup>();

		public MainPage()
		{
			InitializeComponent();
			BindingContext = this;

			for (int i = 0; i < 40; i++)
			{
				StopTimesGroups.Add(new StopTimesGroup
				{
					CodMode = "8",
					Line = "BUG",
					Name = "Slow",
					StopTimeNames = new List<StopTimeName>
				{
					new StopTimeName
					{
						Name = "BUG",
						Times = new List<string>
						{
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8",
							"8"
						}
					},
					 new StopTimeName
					{
						Name = "SLOW",
						Times = new List<string>
						{
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9",
							"9"
						}
					}
				}
				});

			}
		}
	}

	public class StopTimesGroup
	{
		public StopTimesGroup()
		{
		}

		public StopTimesGroup(string name, string codMode, string line, List<StopTimeName> stopTimeNames)
		{
			Name = name;
			CodMode = codMode;
			Line = line;
			StopTimeNames = stopTimeNames;
		}

		public string Name { get; set; }
		public string CodMode { get; set; }
		public string Line { get; set; }
		public List<StopTimeName> StopTimeNames { get; set; } = new List<StopTimeName>();
	}

	public class StopTimeName
	{
		public string Name { get; set; }
		public List<string> Times { get; set; } = new List<string>();
	}
}