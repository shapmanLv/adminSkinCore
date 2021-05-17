const getters = {
  sidebar: state => state.app.sidebar,
  device: state => state.app.device,
  name: state => state.user.name,
  account: state => state.user.account,
  language: state => state.app.language,
  menuTree: state => state.menu.menuTree,
  hasSettingRouter: state => state.app.hasSettingRouter,
  visitedViews: state => state.tagsView.visitedViews,
  cachedViews: state => state.tagsView.cachedViews
}
export default getters
