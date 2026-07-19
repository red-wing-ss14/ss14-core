using Content.Client.Stylesheets;
using Content.Client.Stylesheets.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using static Content.Client.Stylesheets.StylesheetHelpers;

namespace Content.Client._RW.Stylesheets;

[CommonSheetlet]
public sealed class DepartmentButtonsSheetlet : Sheetlet<NanotrasenStylesheet>
{
    private static readonly Color ButtonColorCentralCommand = Color.FromHex("#0c344d");
    private static readonly Color ButtonColorCommand = Color.FromHex("#334E6D");
    private static readonly Color ButtonColorSecurity = Color.FromHex("#DE3A3A");
    private static readonly Color ButtonColorMedical = Color.FromHex("#52B4E9");
    private static readonly Color ButtonColorEngineering = Color.FromHex("#EFB341");
    private static readonly Color ButtonColorCargo = Color.FromHex("#A46106");
    private static readonly Color ButtonColorScience = Color.FromHex("#D381C9");
    private static readonly Color ButtonColorSilicon = Color.FromHex("#D381C9");
    private static readonly Color ButtonColorCivilian = Color.FromHex("#40A166");
    private static readonly Color ButtonColorJustice = Color.FromHex("#8E3D3D");
    private static readonly Color ButtonColorSpecific = Color.FromHex("#969696");
    private static readonly Color ButtonColorAntagonist = Color.FromHex("#7F4141");

    public override StyleRule[] GetRules(NanotrasenStylesheet sheet, object config)
    {
        return
        [
            E<Button>().Class("ButtonColorCentralCommandDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorCentralCommand),
            E<Button>().Class("ButtonColorCommandDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorCommand),
            E<Button>().Class("ButtonColorSecurityDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorSecurity),
            E<Button>().Class("ButtonColorMedicalDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorMedical),
            E<Button>().Class("ButtonColorEngineeringDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorEngineering),
            E<Button>().Class("ButtonColorScienceDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorScience),
            E<Button>().Class("ButtonColorSiliconDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorSilicon),
            E<Button>().Class("ButtonColorCargoDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorCargo),
            E<Button>().Class("ButtonColorCivilianDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorCivilian),
            E<Button>().Class("ButtonColorJusticeDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorJustice),
            E<Button>().Class("ButtonColorSpecificDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorSpecific),
            E<Button>().Class("ButtonColorAntagonistDepartment").Prop(Control.StylePropertyModulateSelf, ButtonColorAntagonist)
        ];
    }
}
