using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

namespace CoxAutomotive.ProgrammingChallenge.Models
{
    /// <summary>
    /// AnswerResponse
    /// </summary>
    [DataContract]
    public class AnswerResponse :  IEquatable<AnswerResponse>, IValidatableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnswerResponse" /> class.
        /// </summary>
        /// <param name="success">success.</param>
        /// <param name="message">message.</param>
        /// <param name="totalMilliseconds">totalMilliseconds.</param>
        public AnswerResponse(bool? success = default(bool?), string message = default(string), int? totalMilliseconds = default(int?))
        {
            this.Success = success;
            this.Message = message;
            this.TotalMilliseconds = totalMilliseconds;
        }
        
        /// <summary>
        /// Gets or Sets Success
        /// </summary>
        [DataMember(Name="success", EmitDefaultValue=false)]
        public bool? Success { get; set; }

        /// <summary>
        /// Gets or Sets Message
        /// </summary>
        [DataMember(Name="message", EmitDefaultValue=false)]
        public string Message { get; set; }

        /// <summary>
        /// Gets or Sets TotalMilliseconds
        /// </summary>
        [DataMember(Name="totalMilliseconds", EmitDefaultValue=false)]
        public int? TotalMilliseconds { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class AnswerResponse {\n");
            sb.Append("  Success: ").Append(Success).Append("\n");
            sb.Append("  Message: ").Append(Message).Append("\n");
            sb.Append("  TotalMilliseconds: ").Append(TotalMilliseconds).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as AnswerResponse);
        }

        /// <summary>
        /// Returns true if AnswerResponse instances are equal
        /// </summary>
        /// <param name="input">Instance of AnswerResponse to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(AnswerResponse input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.Success == input.Success ||
                    (this.Success != null &&
                    this.Success.Equals(input.Success))
                ) && 
                (
                    this.Message == input.Message ||
                    (this.Message != null &&
                    this.Message.Equals(input.Message))
                ) && 
                (
                    this.TotalMilliseconds == input.TotalMilliseconds ||
                    (this.TotalMilliseconds != null &&
                    this.TotalMilliseconds.Equals(input.TotalMilliseconds))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.Success != null)
                    hashCode = hashCode * 59 + this.Success.GetHashCode();
                if (this.Message != null)
                    hashCode = hashCode * 59 + this.Message.GetHashCode();
                if (this.TotalMilliseconds != null)
                    hashCode = hashCode * 59 + this.TotalMilliseconds.GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// To validate all properties of the instance
        /// </summary>
        /// <param name="validationContext">Validation context</param>
        /// <returns>Validation Result</returns>
        IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }

}
