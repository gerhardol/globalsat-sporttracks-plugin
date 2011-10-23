﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// This source code was auto-generated by xsd, Version=2.0.50727.42.
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
[System.Xml.Serialization.XmlRootAttribute("gpx", Namespace="http://www.topografix.com/GPX/1/1", IsNullable=false)]
public partial class gpxType {
    
    private metadataType metadataField;
    
    private wptType[] wptField;
    
    private rteType[] rteField;
    
    private trkType[] trkField;
    
    private extensionsType extensionsField;
    
    private string versionField;
    
    private string creatorField;
    
    public gpxType() {
        this.versionField = "1.1";
    }
    
    /// <remarks/>
    public metadataType metadata {
        get {
            return this.metadataField;
        }
        set {
            this.metadataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("wpt")]
    public wptType[] wpt {
        get {
            return this.wptField;
        }
        set {
            this.wptField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("rte")]
    public rteType[] rte {
        get {
            return this.rteField;
        }
        set {
            this.rteField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("trk")]
    public trkType[] trk {
        get {
            return this.trkField;
        }
        set {
            this.trkField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string version {
        get {
            return this.versionField;
        }
        set {
            this.versionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string creator {
        get {
            return this.creatorField;
        }
        set {
            this.creatorField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class metadataType {
    
    private string nameField;
    
    private string descField;
    
    private personType authorField;
    
    private copyrightType copyrightField;
    
    private linkType[] linkField;
    
    private System.DateTime timeField;
    
    private bool timeFieldSpecified;
    
    private string keywordsField;
    
    private boundsType boundsField;
    
    private extensionsType extensionsField;
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public string desc {
        get {
            return this.descField;
        }
        set {
            this.descField = value;
        }
    }
    
    /// <remarks/>
    public personType author {
        get {
            return this.authorField;
        }
        set {
            this.authorField = value;
        }
    }
    
    /// <remarks/>
    public copyrightType copyright {
        get {
            return this.copyrightField;
        }
        set {
            this.copyrightField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("link")]
    public linkType[] link {
        get {
            return this.linkField;
        }
        set {
            this.linkField = value;
        }
    }
    
    /// <remarks/>
    public System.DateTime time {
        get {
            return this.timeField;
        }
        set {
            this.timeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool timeSpecified {
        get {
            return this.timeFieldSpecified;
        }
        set {
            this.timeFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string keywords {
        get {
            return this.keywordsField;
        }
        set {
            this.keywordsField = value;
        }
    }
    
    /// <remarks/>
    public boundsType bounds {
        get {
            return this.boundsField;
        }
        set {
            this.boundsField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class personType {
    
    private string nameField;
    
    private emailType emailField;
    
    private linkType linkField;
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public emailType email {
        get {
            return this.emailField;
        }
        set {
            this.emailField = value;
        }
    }
    
    /// <remarks/>
    public linkType link {
        get {
            return this.linkField;
        }
        set {
            this.linkField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class emailType {
    
    private string idField;
    
    private string domainField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string domain {
        get {
            return this.domainField;
        }
        set {
            this.domainField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class trksegType {
    
    private wptType[] trkptField;
    
    private extensionsType extensionsField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("trkpt")]
    public wptType[] trkpt {
        get {
            return this.trkptField;
        }
        set {
            this.trkptField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class wptType {
    
    private decimal eleField;
    
    private bool eleFieldSpecified;
    
    private System.DateTime timeField;
    
    private bool timeFieldSpecified;
    
    private decimal magvarField;
    
    private bool magvarFieldSpecified;
    
    private decimal geoidheightField;
    
    private bool geoidheightFieldSpecified;
    
    private string nameField;
    
    private string cmtField;
    
    private string descField;
    
    private string srcField;
    
    private linkType[] linkField;
    
    private string symField;
    
    private string typeField;
    
    private fixType fixField;
    
    private bool fixFieldSpecified;
    
    private string satField;
    
    private decimal hdopField;
    
    private bool hdopFieldSpecified;
    
    private decimal vdopField;
    
    private bool vdopFieldSpecified;
    
    private decimal pdopField;
    
    private bool pdopFieldSpecified;
    
    private decimal ageofdgpsdataField;
    
    private bool ageofdgpsdataFieldSpecified;
    
    private string dgpsidField;
    
    private extensionsType extensionsField;
    
    private decimal latField;
    
    private decimal lonField;
    
    /// <remarks/>
    public decimal ele {
        get {
            return this.eleField;
        }
        set {
            this.eleField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool eleSpecified {
        get {
            return this.eleFieldSpecified;
        }
        set {
            this.eleFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public System.DateTime time {
        get {
            return this.timeField;
        }
        set {
            this.timeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool timeSpecified {
        get {
            return this.timeFieldSpecified;
        }
        set {
            this.timeFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public decimal magvar {
        get {
            return this.magvarField;
        }
        set {
            this.magvarField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool magvarSpecified {
        get {
            return this.magvarFieldSpecified;
        }
        set {
            this.magvarFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public decimal geoidheight {
        get {
            return this.geoidheightField;
        }
        set {
            this.geoidheightField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool geoidheightSpecified {
        get {
            return this.geoidheightFieldSpecified;
        }
        set {
            this.geoidheightFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public string cmt {
        get {
            return this.cmtField;
        }
        set {
            this.cmtField = value;
        }
    }
    
    /// <remarks/>
    public string desc {
        get {
            return this.descField;
        }
        set {
            this.descField = value;
        }
    }
    
    /// <remarks/>
    public string src {
        get {
            return this.srcField;
        }
        set {
            this.srcField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("link")]
    public linkType[] link {
        get {
            return this.linkField;
        }
        set {
            this.linkField = value;
        }
    }
    
    /// <remarks/>
    public string sym {
        get {
            return this.symField;
        }
        set {
            this.symField = value;
        }
    }
    
    /// <remarks/>
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    public fixType fix {
        get {
            return this.fixField;
        }
        set {
            this.fixField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool fixSpecified {
        get {
            return this.fixFieldSpecified;
        }
        set {
            this.fixFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
    public string sat {
        get {
            return this.satField;
        }
        set {
            this.satField = value;
        }
    }
    
    /// <remarks/>
    public decimal hdop {
        get {
            return this.hdopField;
        }
        set {
            this.hdopField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool hdopSpecified {
        get {
            return this.hdopFieldSpecified;
        }
        set {
            this.hdopFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public decimal vdop {
        get {
            return this.vdopField;
        }
        set {
            this.vdopField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool vdopSpecified {
        get {
            return this.vdopFieldSpecified;
        }
        set {
            this.vdopFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public decimal pdop {
        get {
            return this.pdopField;
        }
        set {
            this.pdopField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool pdopSpecified {
        get {
            return this.pdopFieldSpecified;
        }
        set {
            this.pdopFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    public decimal ageofdgpsdata {
        get {
            return this.ageofdgpsdataField;
        }
        set {
            this.ageofdgpsdataField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIgnoreAttribute()]
    public bool ageofdgpsdataSpecified {
        get {
            return this.ageofdgpsdataFieldSpecified;
        }
        set {
            this.ageofdgpsdataFieldSpecified = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
    public string dgpsid {
        get {
            return this.dgpsidField;
        }
        set {
            this.dgpsidField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal lat {
        get {
            return this.latField;
        }
        set {
            this.latField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal lon {
        get {
            return this.lonField;
        }
        set {
            this.lonField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class linkType {
    
    private string textField;
    
    private string typeField;
    
    private string hrefField;
    
    /// <remarks/>
    public string text {
        get {
            return this.textField;
        }
        set {
            this.textField = value;
        }
    }
    
    /// <remarks/>
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute(DataType="anyURI")]
    public string href {
        get {
            return this.hrefField;
        }
        set {
            this.hrefField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public enum fixType {
    
    /// <remarks/>
    none,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("2d")]
    Item2d,
    
    /// <remarks/>
    [System.Xml.Serialization.XmlEnumAttribute("3d")]
    Item3d,
    
    /// <remarks/>
    dgps,
    
    /// <remarks/>
    pps,
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class extensionsType {
    
    private System.Xml.XmlElement[] anyField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAnyElementAttribute()]
    public System.Xml.XmlElement[] Any {
        get {
            return this.anyField;
        }
        set {
            this.anyField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class trkType {
    
    private string nameField;
    
    private string cmtField;
    
    private string descField;
    
    private string srcField;
    
    private linkType[] linkField;
    
    private string numberField;
    
    private string typeField;
    
    private extensionsType extensionsField;
    
    private trksegType[] trksegField;
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public string cmt {
        get {
            return this.cmtField;
        }
        set {
            this.cmtField = value;
        }
    }
    
    /// <remarks/>
    public string desc {
        get {
            return this.descField;
        }
        set {
            this.descField = value;
        }
    }
    
    /// <remarks/>
    public string src {
        get {
            return this.srcField;
        }
        set {
            this.srcField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("link")]
    public linkType[] link {
        get {
            return this.linkField;
        }
        set {
            this.linkField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
    public string number {
        get {
            return this.numberField;
        }
        set {
            this.numberField = value;
        }
    }
    
    /// <remarks/>
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("trkseg")]
    public trksegType[] trkseg {
        get {
            return this.trksegField;
        }
        set {
            this.trksegField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class rteType {
    
    private string nameField;
    
    private string cmtField;
    
    private string descField;
    
    private string srcField;
    
    private linkType[] linkField;
    
    private string numberField;
    
    private string typeField;
    
    private extensionsType extensionsField;
    
    private wptType[] rteptField;
    
    /// <remarks/>
    public string name {
        get {
            return this.nameField;
        }
        set {
            this.nameField = value;
        }
    }
    
    /// <remarks/>
    public string cmt {
        get {
            return this.cmtField;
        }
        set {
            this.cmtField = value;
        }
    }
    
    /// <remarks/>
    public string desc {
        get {
            return this.descField;
        }
        set {
            this.descField = value;
        }
    }
    
    /// <remarks/>
    public string src {
        get {
            return this.srcField;
        }
        set {
            this.srcField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("link")]
    public linkType[] link {
        get {
            return this.linkField;
        }
        set {
            this.linkField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="nonNegativeInteger")]
    public string number {
        get {
            return this.numberField;
        }
        set {
            this.numberField = value;
        }
    }
    
    /// <remarks/>
    public string type {
        get {
            return this.typeField;
        }
        set {
            this.typeField = value;
        }
    }
    
    /// <remarks/>
    public extensionsType extensions {
        get {
            return this.extensionsField;
        }
        set {
            this.extensionsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("rtept")]
    public wptType[] rtept {
        get {
            return this.rteptField;
        }
        set {
            this.rteptField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class boundsType {
    
    private decimal minlatField;
    
    private decimal minlonField;
    
    private decimal maxlatField;
    
    private decimal maxlonField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal minlat {
        get {
            return this.minlatField;
        }
        set {
            this.minlatField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal minlon {
        get {
            return this.minlonField;
        }
        set {
            this.minlonField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal maxlat {
        get {
            return this.maxlatField;
        }
        set {
            this.maxlatField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public decimal maxlon {
        get {
            return this.maxlonField;
        }
        set {
            this.maxlonField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.42")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.topografix.com/GPX/1/1")]
public partial class copyrightType {
    
    private string yearField;
    
    private string licenseField;
    
    private string authorField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="gYear")]
    public string year {
        get {
            return this.yearField;
        }
        set {
            this.yearField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(DataType="anyURI")]
    public string license {
        get {
            return this.licenseField;
        }
        set {
            this.licenseField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string author {
        get {
            return this.authorField;
        }
        set {
            this.authorField = value;
        }
    }
}
