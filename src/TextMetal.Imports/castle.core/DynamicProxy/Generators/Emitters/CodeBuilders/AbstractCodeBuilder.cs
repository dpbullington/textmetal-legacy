// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Reflection.Emit;

using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Castle.DynamicProxy.Generators.Emitters.CodeBuilders
{
	using System;

	public abstract class AbstractCodeBuilder
	{
		#region Constructors/Destructors

		protected AbstractCodeBuilder(ILGenerator generator)
		{
			this.generator = generator;
			this.stmts = new List<Statement>();
			this.ilmarkers = new List<Reference>();
			this.isEmpty = true;
		}

		#endregion

		#region Fields/Constants

		private readonly ILGenerator generator;
		private readonly List<Reference> ilmarkers;
		private readonly List<Statement> stmts;
		private bool isEmpty;

		#endregion

		//NOTE: should we make this obsolete if no one is using it?

		#region Properties/Indexers/Events

		public /*protected internal*/ ILGenerator Generator
		{
			get
			{
				return this.generator;
			}
		}

		internal bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}
		}

		#endregion

		#region Methods/Operators

		public AbstractCodeBuilder AddExpression(Expression expression)
		{
			return this.AddStatement(new ExpressionStatement(expression));
		}

		public AbstractCodeBuilder AddStatement(Statement stmt)
		{
			this.SetNonEmpty();
			this.stmts.Add(stmt);
			return this;
		}

		public LocalReference DeclareLocal(Type type)
		{
			var local = new LocalReference(type);
			this.ilmarkers.Add(local);
			return local;
		}

		internal void Generate(IMemberEmitter member, ILGenerator il)
		{
			foreach (var local in this.ilmarkers)
				local.Generate(il);

			foreach (var stmt in this.stmts)
				stmt.Emit(member, il);
		}

		public /*protected internal*/ void SetNonEmpty()
		{
			this.isEmpty = false;
		}

		#endregion
	}
}