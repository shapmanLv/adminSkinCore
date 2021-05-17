import { menu_en } from './modules/menu'
import { login_en } from './modules/login'
import { authorizeApi_en } from './modules/authorizeApi'
import { role_en } from './modules/role'
import { user_en } from './modules/user'
import { auth401_en } from './modules/401'
import { auth404_en } from './modules/404'

const en = {
  ...menu_en,
  ...login_en,
  ...authorizeApi_en,
  ...role_en,
  ...user_en,
  ...auth401_en,
  ...auth404_en,
  router: {
    home: '首页',
    permissionGroup: 'authorization',
    menu: 'menu management',
    authorizeApi: 'api management',
    role: 'role management',
    user: 'user management',
    systemtestGroup: 'system test',
    test1: 'test1',
    test2: 'test2'
  },
  app: {
    home: 'home',
    logout: 'logout',
    updatePassword: 'update password',
    title: 'XXX System'
  },
  btn: {
    add: 'add',
    search: 'search',
    batchRemove: 'batch remove',
    cancel: 'cancel',
    submit: 'submit',
    confirm: 'confirm',
    reset: 'reset',
    scanAdd: 'Scan the API and add in bulk'
  },
  removeMessage: {
    removeConfirmContent: 'This action will permanently delete the record. Do you want to continue?',
    removeConfirmTitle: 'hint',
    batchRemoveConfirmContent: 'This action will permanently delete these records. Do you want to continue?',
    notSelect: 'Select the record you want to delete first'
  },
  notify: {
    title: 'Notification',
    success: 'Successfully',
    error: 'Error'
  },
  form: {
    addTitle: 'Add',
    updateTitle: 'Update'
  },
  passwordUpdateform: {
    originalPassword: 'original password',
    newPassword: 'new password',
    passwordAgain: 'password again'
  }
}

export default en
