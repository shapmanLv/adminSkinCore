<template>
  <div class="navbar">
    <template v-if="device === 'mobile'">
      <hamburger :is-active="sidebar.opened" class="hamburger-container" @toggleClick="toggleSideBar" />
    </template>
    <logo class="hamburger-container" />

    <template v-if="device !== 'mobile'">
      <breadcrumb class="breadcrumb-container" />
    </template>

    <div class="right-menu">
      <template v-if="device !== 'mobile'">
        <!-- <langSelect id="langSelect" class="right-menu-item hover-effect" /> -->

        <screenfull id="screenfull" class="right-menu-item hover-effect" />
      </template>

      <el-dropdown
        class="avatar-container right-menu-item hover-effect"
        trigger="click"
      >

        <div class="avatar-wrapper">
          <img src="@/assets/avatar/boy.png">
          <div style="display: inline-block">
            {{ name }}
            <i class="el-icon-caret-bottom" />
          </div>
        </div>
        <div style="clear: both" />

        <el-dropdown-menu slot="dropdown" class="user-dropdown">
          <router-link to="/">
            <el-dropdown-item>
              首页
            </el-dropdown-item>
          </router-link>
          <el-dropdown-item @click.native="showPasswordUpdateFormDialog()">
            修改密码
          </el-dropdown-item>
          <el-dropdown-item divided @click.native="logout">
            <span style="display: block">退出登录</span>
          </el-dropdown-item>
        </el-dropdown-menu>
      </el-dropdown>
    </div>

    <!-- 密码修改表单 -->
    <el-dialog
      :visible.sync="dialogVisible"
      :close-on-click-modal="true"
      :modal-append-to-body="false"
    >
      <el-form
        ref="formData"
        label-width="120px"
        :model="formData"
        :rules="formRules"
      >
        <el-form-item
          label="原密码"
          prop="originalPassword"
        >
          <el-input
            v-model="formData.originalPassword"
            type="password"
            maxlength="20"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          label="新密码"
          prop="newPassword"
        >
          <el-input
            v-model="formData.newPassword"
            type="password"
            autocomplete="new-password"
            maxlength="20"
            show-word-limit
          />
        </el-form-item>
        <el-form-item
          label="再输入一次"
          prop="passwordAgain"
        >
          <el-input
            v-model="formData.passwordAgain"
            type="password"
            autocomplete="new-password"
            maxlength="20"
            show-word-limit
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native="closePasswordUpdateFormDialog">取消</el-button>
        <el-button
          type="primary"
          :loading="formLoading"
          @click.native="submit('formData')"
        >确定</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { mapGetters } from 'vuex'
import Breadcrumb from '@/components/Breadcrumb'
import Hamburger from '@/components/Hamburger'
import Screenfull from '@/components/Screenfull'
import LangSelect from '@/components/LangSelect'
import { UpdatePassword } from '@/api/user'
import Logo from './Sidebar/Logo'

export default {
  components: {
    Screenfull,
    Breadcrumb,
    LangSelect,
    Logo,
    Hamburger
  },
  data() {
    const validatePasswordAgain = (rule, value, callback) => {
      if (this.formData.newPassword === value) {
        callback()
      }

      callback(new Error('两次输入的密码不一致'))
    }
    const validatePassword = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入密码'))
      }
    }
    return {
      dialogVisible: false,
      formLoading: false,
      formData: {
        originalPassword: '',
        newPassword: '',
        passwordAgain: ''
      },
      formRules: {
        originalPassword: [
          { required: true, validator: validatePassword, trigger: 'blur' }
        ],
        newPassword: [
          { required: true, validator: validatePassword, trigger: 'blur' }
        ],
        passwordAgain: [
          { required: true, validator: validatePasswordAgain, trigger: 'blur' }
        ]
      },
      pworiginal: 'pworiginal'
    }
  },
  computed: {
    ...mapGetters(['sidebar', 'avatar', 'name', 'device', 'account'])
  },
  methods: {
    toggleSideBar() {
      this.$store.dispatch('app/toggleSideBar')
    },
    async logout() {
      await this.$store.dispatch('user/logout')
      this.$store.dispatch('app/updateRouterSettingState', false)
      this.$router.push(`/login?redirect=/`)
    },
    closePasswordUpdateFormDialog() {
      this.dialogVisible = false
      setTimeout(() => {
        this.$refs.pwdInput.clear()
      }, 5)
    },
    showPasswordUpdateFormDialog() {
      this.dialogVisible = true
      this.formData.originalPassword = ''
      this.formData.newPassword = ''
      this.formData.passwordAgain = ''
      setTimeout(() => {
        this.$refs.pwdInput.clear()
      }, 5)
    },
    submit(formname) {
      this.$refs[formname].validate((valid) => {
        if (valid) {
          console.log(this.formData)
          this.formLoading = true
          UpdatePassword(this.formData)
            .then((res) => {
              if (res.code === 200) {
                this.$notify({
                  title: '操作通知',
                  message: '成功',
                  type: 'success',
                  duration: 2000
                })
                this.closePasswordUpdateFormDialog()
              } else {
                this.$notify.error({
                  title: '操作通知',
                  message: res.msg,
                  duration: 2000
                })
              }
              this.formLoading = false
            })
            .catch(() => {
              this.formLoading = false
            })
        }
      })
    }
  }
}
</script>

<style lang="scss" scoped>
.navbar {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: #262f3e; /*#fff*/
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);

  .hamburger-container {
    line-height: 46px;
    height: 100%;
    float: left;
    cursor: pointer;
    transition: background 0.3s;
    -webkit-tap-highlight-color: transparent;

    &:hover {
      background: rgba(0, 0, 0, 0.025);
    }
  }

  .breadcrumb-container {
    float: left;
  }

  .right-menu {
    float: right;
    height: 100%;
    line-height: 50px;

    &:focus {
      outline: none;
    }

    .right-menu-item {
      display: inline-block;
      padding: 0 16px;
      height: 100%;
      font-size: 14px;
      color: #d3d3d3; /* #5a5e66 */
      vertical-align: text-bottom;

      &.hover-effect {
        cursor: pointer;
        transition: background 0.3s;

        &:hover {
          background: rgba(0, 0, 0, 0.025);
        }
      }
    }

    .avatar-container {
      margin-right: 30px;

      .avatar-wrapper {
        margin-top: 0px;
        position: relative;

        img {
          float: left;
          height: 25px;
          margin-top: 12px;
          margin-right: 8px;
        }

        .el-icon-caret-bottom {
          cursor: pointer;
          position: absolute;
          right: -20px;
          top: 25px;
          font-size: 12px;
        }
      }
    }
  }
}
</style>
