// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org.
// ****************************************************************

using System;
using System.Collections;

namespace NUnit.Util
{
	/// <summary>
	/// 	Summary description for ProjectConfigCollection.
	/// </summary>
	public class ProjectConfigCollection : CollectionBase
	{
		#region Constructors/Destructors

		public ProjectConfigCollection(NUnitProject project)
		{
			this.project = project;
		}

		#endregion

		#region Fields/Constants

		protected NUnitProject project;

		#endregion

		#region Properties/Indexers/Events

		public ProjectConfig this[int index]
		{
			get
			{
				return (ProjectConfig)this.InnerList[index];
			}
		}

		public ProjectConfig this[string name]
		{
			get
			{
				int index = this.IndexOf(name);
				return index >= 0 ? (ProjectConfig)this.InnerList[index] : null;
			}
		}

		#endregion

		#region Methods/Operators

		public void Add(ProjectConfig config)
		{
			this.List.Add(config);
			config.Project = this.project;
		}

		public void Add(string name)
		{
			this.Add(new ProjectConfig(name));
		}

		public bool Contains(ProjectConfig config)
		{
			return this.InnerList.Contains(config);
		}

		public bool Contains(string name)
		{
			return this.IndexOf(name) >= 0;
		}

		private int IndexOf(string name)
		{
			for (int index = 0; index < this.InnerList.Count; index++)
			{
				ProjectConfig config = (ProjectConfig)this.InnerList[index];
				if (config.Name == name)
					return index;
			}

			return -1;
		}

		protected override void OnInsertComplete(int index, object obj)
		{
			if (this.project != null)
			{
				this.project.IsDirty = true;
				if (this.Count == 1)
					this.project.HasChangesRequiringReload = true;
			}
		}

		protected override void OnRemove(int index, object value)
		{
			if (this.project != null)
			{
				ProjectConfig config = value as ProjectConfig;
				this.project.IsDirty = true;
				if (config.Name == this.project.ActiveConfigName)
					this.project.HasChangesRequiringReload = true;
			}
		}

		public void Remove(string name)
		{
			int index = this.IndexOf(name);
			if (index >= 0)
				this.RemoveAt(index);
		}

		#endregion
	}
}