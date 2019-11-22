using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Brierley.FrameWork.Common;

namespace Brierley.FrameWork.Data.DomainModel
{
    [PetaPoco.ExplicitColumns]
    [PetaPoco.PrimaryKey("Id", sequenceName = "hibernate_sequence")]
    [PetaPoco.TableName("LW_RemoteAssembly")]
    public class RemoteAssembly : LWCoreObjectBase
    {
        public class ComponentReference
        {
            public string Name;
            public string ClassName;
            public System.Reflection.Assembly Assembly;
        }

        [PetaPoco.Column(IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the AssemblyName for the component
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the AssemblyFileName for the component
        /// </summary>
        [PetaPoco.Column(Length = 255, IsNullable = false)]
        public string AssemblyFileName { get; set; }

        private byte[] _assemblyBytes;
        /// <summary>
        /// Gets or sets the Assembly for the current ClientDataModel
        /// </summary>
        [PetaPoco.Column]
        public byte[] Assembly
        {
            get
            {
                return _assemblyBytes;
            }
            set
            {
                _assemblyBytes = value;
                _loadedAssembly = null;
                if (_assemblyBytes != null)
                    PopulateReferences();
            }
        }

        private System.Reflection.Assembly _loadedAssembly;
        public System.Reflection.Assembly LoadedAssembly
        {
            get
            {
                if(_loadedAssembly == null && _assemblyBytes != null)
                {
                    PopulateReferences();
                }
                return _loadedAssembly;
            }
            private set
            {
                _loadedAssembly = value;
            }
        }

        private Dictionary<CustomComponentTypeEnum, List<ComponentReference>> _references;
        private void PopulateReferences()
        {
            _references = new Dictionary<CustomComponentTypeEnum, List<ComponentReference>>();
            foreach (CustomComponentTypeEnum comp in Enum.GetValues(typeof(CustomComponentTypeEnum)))
            {
                _references.Add(comp, new List<ComponentReference>());
            }

            if (_loadedAssembly == null)
            {
                _loadedAssembly = System.Reflection.Assembly.Load(_assemblyBytes);
            }

            Type[] exportedTypes = null;
            try
            {
                exportedTypes = _loadedAssembly.GetExportedTypes();
            }
            catch
            { return; }

            if (exportedTypes != null && exportedTypes.Length > 0)
            {
                foreach (Type type in exportedTypes)
                {
                    CustomComponentTypeEnum? componentType = null;
                    if (typeof(bScript.Expression).IsAssignableFrom(type))
                        componentType = CustomComponentTypeEnum.BScript;

                    if (typeof(Rules.RuleBase).IsAssignableFrom(type))
                        componentType = CustomComponentTypeEnum.Rule;

                    if (typeof(Interfaces.IXsltPostProcessor).IsAssignableFrom(type))
                        componentType = CustomComponentTypeEnum.XsltPostProcessor;

                    if (typeof(ClientDataObject).IsAssignableFrom(type))
                        componentType = CustomComponentTypeEnum.DataModel;

                    //componentType = CustomComponentTypeEnum.Form? has something to do with bookmarks

                    // If it matches something we're looking for, then add it
                    if (componentType.HasValue)
                    {
                        _references[componentType.Value].Add(new ComponentReference()
                        {
                            Name = type.Name,
                            ClassName = type.AssemblyQualifiedName,
                            Assembly = _loadedAssembly
                        });
                    }
                }
            }
        }

        public List<ComponentReference> FindComponents(CustomComponentTypeEnum componentType)
        {
            return _references[componentType];
        }

        public ComponentReference GetReferenceByName(CustomComponentTypeEnum componentType, string name)
        {
            foreach (var reference in _references[componentType])
            {
                if (reference.Name.ToLower() == name.ToLower())
                    return reference;
            }

            return null;
        }

        public RemoteAssembly Clone()
        {
            return Clone(new RemoteAssembly());
        }

        public RemoteAssembly Clone(RemoteAssembly dest)
        {
            dest.AssemblyFileName = AssemblyFileName;
            dest.AssemblyName = AssemblyName;
            dest.Assembly = Assembly;
            return (RemoteAssembly)base.Clone(dest);
        }
    }
}
