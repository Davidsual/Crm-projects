using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reply.Iveco.LeadManagement.Presenter.Model
{
    public partial class Constraints
    {

        private ConstraintsConstraint constraintField;

        /// <remarks/>
        public ConstraintsConstraint Constraint
        {
            get
            {
                return this.constraintField;
            }
            set
            {
                this.constraintField = value;
            }
        }
    }

    public partial class ConstraintsConstraint
    {

        private ConstraintsConstraintExpression expressionField;

        /// <remarks/>
        public ConstraintsConstraintExpression Expression
        {
            get
            {
                return this.expressionField;
            }
            set
            {
                this.expressionField = value;
            }
        }
    }

    public partial class ConstraintsConstraintExpression
    {

        private string bodyField;

        private ConstraintsConstraintExpressionParameters parametersField;

        /// <remarks/>
        public string Body
        {
            get
            {
                return this.bodyField;
            }
            set
            {
                this.bodyField = value;
            }
        }

        /// <remarks/>
        public ConstraintsConstraintExpressionParameters Parameters
        {
            get
            {
                return this.parametersField;
            }
            set
            {
                this.parametersField = value;
            }
        }
    }

    public partial class ConstraintsConstraintExpressionParameters
    {

        private ConstraintsConstraintExpressionParametersParameter parameterField;

        /// <remarks/>
        public ConstraintsConstraintExpressionParametersParameter Parameter
        {
            get
            {
                return this.parameterField;
            }
            set
            {
                this.parameterField = value;
            }
        }
    }

    public partial class ConstraintsConstraintExpressionParametersParameter
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }
}
