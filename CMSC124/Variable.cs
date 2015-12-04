using System;

namespace CMSC124
{
	public class Variable
	{
		public string variableName;
		public string variableValue;
		public string variableType;
		public Variable (string vN, string vV, string vT)
		{	
			this.variableName = vN;
			this.variableValue = vV;
			this.variableType = vT;
		}
	}
}

