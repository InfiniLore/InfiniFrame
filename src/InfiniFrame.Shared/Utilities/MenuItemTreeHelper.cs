// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
using System.Collections.Immutable;

namespace InfiniFrame.Utilities;
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
/// <summary>
///     Pure logic for recursively updating immutable menu item trees.
///     Extracted from <see cref="MenuInfiniFrameWindowFeature"/> for testability.
/// </summary>
public static class MenuItemTreeHelper {

    /// <summary>
    ///     Recursively finds a menu item by ID and applies an updater function.
    ///     Returns a new immutable array with the updated item.
    /// </summary>
    public static ImmutableArray<InfiniFrameMenuItem> UpdateItem(
        ImmutableArray<InfiniFrameMenuItem> items,
        string menuItemId,
        Func<InfiniFrameMenuItem, InfiniFrameMenuItem> updater
    ) {
        ImmutableArray<InfiniFrameMenuItem>.Builder builder = items.ToBuilder();

        for (int i = 0; i < builder.Count; i++) {
            if (builder[i].Id == menuItemId) {
                builder[i] = updater(builder[i]);
            } else if (!builder[i].Children.IsDefaultOrEmpty) {
                builder[i] = builder[i] with {
                    Children = UpdateItem(builder[i].Children, menuItemId, updater)
                };
            }
        }

        return builder.ToImmutable();
    }
}
