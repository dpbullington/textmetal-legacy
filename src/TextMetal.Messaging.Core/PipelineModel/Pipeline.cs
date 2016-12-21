/*
	Copyright ©2002-2017 Daniel P. Bullington (dpbullington@gmail.com)
	Distributed under the MIT license: http://www.opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace TextMetal.Messaging.Core.PipelineModel
{
	public abstract class Pipeline : IntegrationComponent, IPipeline
	{
		#region Constructors/Destructors

		protected Pipeline()
		{
		}

		#endregion

		#region Fields/Constants

		private readonly IList<IStage> stages = new List<IStage>();
		private readonly Dictionary<string, Type> stageTypes = new Dictionary<string, Type>();

		#endregion

		#region Properties/Indexers/Events

		private IList<IStage> Stages
		{
			get
			{
				return this.stages;
			}
		}

		private Dictionary<string, Type> StageTypes
		{
			get
			{
				return this.stageTypes;
			}
		}

		#endregion

		#region Methods/Operators

		public virtual void AddStageType(string stageName, Type stageType)
		{
			this.AssertMutable();
			this.StageTypes.Add(stageName, stageType);
		}

		protected virtual void ApplyStageTypes()
		{
			this.AssertMutable();
		}

		public virtual void ClearStageTypes()
		{
			this.AssertMutable();
			this.StageTypes.Clear();
		}

		protected override void CoreInitialize()
		{
			this.ApplyStageTypes();
		}

		protected override void CoreTerminate()
		{
			foreach (IStage stage in this.Stages)
				stage.Terminate();

			this.Stages.Clear();
		}

		public virtual IPipeliningContext CreateContext()
		{
			this.WriteLogSynchronized("PIPELINE: Creating pipeline context on thread '{0}'.", Thread.CurrentThread.ManagedThreadId);

			return new PipeliningContext(Guid.NewGuid());
		}

		public virtual IReadOnlyDictionary<string, Type> GetStageTypes()
		{
			return this.StageTypes;
		}

		public virtual void RemoveStage(string name)
		{
			this.AssertMutable();
			this.StageTypes.Remove(name);
		}

		protected void TrackStageInstance(IStage stage)
		{
			if ((object)stage == null)
				throw new ArgumentNullException("stage");

			this.Stages.Add(stage);
		}

		#endregion
	}
}