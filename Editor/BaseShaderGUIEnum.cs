using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SGP
{
    // Put these in a separate file in case Enum changing with Unity Version
    public enum BlendMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply,
    }
    
    public enum DoubleSidedNormalMode
    {
        Mirror,
        Flip,
        None
    }

    public enum RenderFace
    {
        Both,
        Back,
        Front
    }
    
    public enum MaterialType
    {
        Standard = 0,
        Anisotropic = 1,
        Iridescence = 2,
        SubsurfaceScattering = 3,
        Translucent = 4,
    }
    
    
    
    public enum SpecOcclusionMode
    {
        Off = 0,
        FromAmbientOcclusion = 1,
        FromBentNormalsAndAO = 2,
        FromGI = 3
    }
    public enum SurfaceType
    {
        Opaque,
        Transparent,
    }

    public enum WorkflowMode
    {
        Specular,
        Metallic,
    }

    public enum ZWriteControl
    {
        Auto,
        ForceEnabled,
        ForceDisabled,
    }
    
    public enum QueueControl
    {
        Auto = 0,
        UserOverride = 1
    }
    
    public enum MaterialUpdateType
    {
        CreatedNewMaterial,
        ChangedAssignedShader,
        ModifiedShader,
        ModifiedMaterial
    }
    
}
