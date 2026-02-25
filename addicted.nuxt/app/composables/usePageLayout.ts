/**
 * Composable for consistent page layout styling following the glass panel design system.
 * Provides color values and class strings for containers, panels, and nested elements.
 */
export const usePageLayout = () => {
  // Glass panel color palette
  const colors = {
    // Primary panel background (main content containers)
    primaryPanel: "rgba(0,0,0,0.75)",
    // App bar background
    appBar: "rgba(0,0,0,0.7)",
    // Nested item background (cards inside panels)
    nestedItem: "rgba(255,255,255,0.08)",
    // Expansion panel background
    expansionPanel: "rgba(255,255,255,0.05)",
  };

  // Reusable class combinations for common layouts
  const classes = {
    // Main page container
    pageContainer: "pa-2 pa-sm-4",
    // Primary panel
    primaryPanel: "pa-4 pa-sm-6 mb-4",
    // Nested item (e.g., subtitle cards on mobile)
    nestedItem: "pa-3",
    // Page centered heading
    pageHeading: "text-center mb-4",
    // Page title
    pageTitle: "text-h5 text-sm-h4 font-weight-bold",
    // Page subtitle
    pageSubtitle: "text-body-2 text-sm-body-1 text-medium-emphasis mt-1",
    // Body text
    bodyText: "text-body-2 text-sm-body-1",
    // Section heading
    sectionHeading: "text-h6 mt-4",
  };

  // Maximum container width (matches Vuetify default)
  const maxWidth = "1600px";

  return {
    colors,
    classes,
    maxWidth,
  };
};
