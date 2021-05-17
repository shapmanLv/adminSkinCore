import { menu_zh } from './modules/menu'
import { login_zh } from './modules/login'
import { authorizeApi_zh } from './modules/authorizeApi'
import { role_zh } from './modules/role'
import { user_zh } from './modules/user'
import { auth401_zh } from './modules/401'
import { auth404_zh } from './modules/404'
import { title } from '../settings'

const zh = {
  ...menu_zh,
  ...login_zh,
  ...authorizeApi_zh,
  ...role_zh,
  ...user_zh,
  ...auth401_zh,
  ...auth404_zh,
  router: {
    home: '首页',
    permissionGroup: '权限配置',
    menu: '菜单管理',
    authorizeApi: '接口管理',
    role: '角色管理',
    user: '用户管理',
    systemtestGroup: '系统测试',
    test1: 'test1',
    test2: 'test2'
  },
  app: {
    home: '首页',
    logout: '退出登录',
    updatePassword: '修改密码',
    title: title
  },
  btn: {
    add: '添加',
    search: '搜索',
    batchRemove: '批量删除',
    cancel: '取消',
    submit: '提交',
    confirm: '确定',
    reset: '重置',
    scanAdd: '扫描接口并批量添加'
  },
  removeMessage: {
    removeConfirmContent: '此操作将永久删除该记录, 是否继续?',
    removeConfirmTitle: '提示',
    batchRemoveConfirmContent: '此操作将永久删除这些记录, 是否继续?',
    notSelect: '请先选择要删除的记录'
  },
  notify: {
    title: '操作通知',
    success: '成功',
    error: '失败'
  },
  form: {
    addTitle: '添加',
    updateTitle: '修改'
  },
  passwordUpdateform: {
    originalPassword: '原密码',
    newPassword: '新密码',
    passwordAgain: '再输一次'
  }
}

export default zh
