<template>
  <div class="login-container">
    <el-form
      ref="loginForm"
      :model="loginForm"
      :rules="loginRules"
      class="login-form"
      auto-complete="on"
      label-position="left"
    >
      <div class="title-container">
        <h3 class="title">{{ systemTitle }}登录</h3>

        <div style="clear:both" />
      </div>

      <el-form-item prop="username">
        <span class="svg-container">
          <svg-icon icon-class="user" />
        </span>
        <el-input
          ref="username"
          v-model="loginForm.username"
          placeholder="用户名"
          name="username"
          type="text"
          tabindex="1"
          auto-complete="on"
        />
      </el-form-item>

      <el-form-item prop="password">
        <span class="svg-container">
          <svg-icon icon-class="password" />
        </span>
        <el-input
          :key="passwordType"
          ref="password"
          v-model="loginForm.password"
          placeholder="密码"
          :type="passwordType"
          name="password"
          tabindex="2"
          auto-complete="on"
          @keyup.enter.native="handleLogin"
        />
        <span class="show-pwd" @click="showPwd">
          <svg-icon
            :icon-class="passwordType === 'password' ? 'eye' : 'eye-open'"
          />
        </span>
      </el-form-item>

      <el-form-item prop="verificationCode" style="float: left; width: 68%">
        <span class="svg-container">
          <svg-icon icon-class="icon" />
        </span>
        <el-input
          ref="verificationCode"
          v-model="loginForm.verificationCode"
          placeholder="验证码"
          name="verificationCode"
          type="text"
          tabindex="3"
          auto-complete="on"
          :maxlength="6"
          @input="codeInputChange()"
          @keyup.enter.native="handleLogin"
        />
      </el-form-item>
      <!-- <el-form-item label="" style="float: right;">

      </el-form-item> -->
      <div style="float: right">
        <el-button
          type="primary"
          :disabled="buttonDisabled"
          @click="getVerificationCode()"
        >{{ loginBtnText }}</el-button>
      </div>

      <el-button
        :loading="loading"
        type="primary"
        style="width: 100%; margin-bottom: 30px"
        @click.native.prevent="handleLogin"
      >登录</el-button>
    </el-form>
  </div>
</template>

<script>
import { login, sendVerificationCode } from '@/api/account'
import { title } from '@/settings.js'

export default {
  name: 'Login',
  data() {
    const validateUsername = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入用户名'))
      }
    }
    const validatePassword = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入密码'))
      }
    }
    const validateVerificationCode = (rule, value, callback) => {
      if (value) {
        if (isNaN(value)) {
          callback(new Error('验证码必须是数字'))
        }

        if (value.length === 6) {
          callback()
        } else {
          callback(new Error('验证码格式不符合，长度应该是6位'))
        }
      } else {
        callback(new Error('请输入验证码'))
      }
    }
    return {
      loginForm: {
        username: '',
        password: '',
        verificationCode: ''
      },
      loginRules: {
        username: [{ required: true, trigger: 'blur', validator: validateUsername }],
        password: [{ required: true, trigger: 'blur', validator: validatePassword }],
        verificationCode: [{ required: true, trigger: 'blur', validator: validateVerificationCode }]
      },
      loading: false,
      passwordType: 'password',
      redirect: undefined,

      /** 手机短信验证码 */
      passwordErrorCount: 0, // 密码错误次数统计
      verificationCode: '', // 验证码
      phone: '', // 手机号  在用户登录，发现是密码错误的时候，后端会返回手机号回来，同时这个也是用户自己修改后的手机号
      lastLoginName: '', // 上一次登录错误时，登录的账户的用户名
      showDialog: false,

      buttonText: '', // 59s后重试
      second: 60, // parseInt(sessionStorage.getItem('second') || 60),
      intervalTemp: null,
      buttonDisabled: false,
      systemTitle: title
    }
  },
  computed: {
    loginBtnText() {
      if (this.buttonText === '') {
        return '获取验证码'
      } else {
        return this.buttonText
      }
    }
  },
  watch: {
    $route: {
      handler: function(route) {
        this.redirect = route.query && route.query.redirect
      },
      immediate: true
    }
  },
  methods: {
    showPwd() {
      if (this.passwordType === 'password') {
        this.passwordType = ''
      } else {
        this.passwordType = 'password'
      }
      this.$nextTick(() => {
        this.$refs.password.focus()
      })
    },
    /** 登录 */
    handleLogin() {
      this.$refs.loginForm.validate(valid => {
        if (valid) {
          this.loading = true
          login({ account: this.loginForm.username,
            password: this.loginForm.password,
            captcha: this.loginForm.verificationCode,
            clientIp: localStorage.getItem('ip') }).then(res => {
            if (res.code === 200) { // 登录成功
              this.$notify({
                title: '操作通知',
                message: res.msg,
                type: 'success',
                duration: 2000
              })

              /** 提交请求获取用户信息 */
              this.$store.dispatch('user/getInfo', this.loginForm)
                .then(() => {
                  this.$router.push({ path: this.redirect || '/', query: this.otherQuery })
                  this.loading = false
                })
                .catch(() => {
                  this.loading = false
                })

              // this.$store.dispatch('menu/buildUserMenuTree')
            } else {
              this.loading = false
              this.$notify.error({
                title: '操作通知',
                message: res.msg,
                duration: 2000
              })

              this.clearGetCodeBtnTextInterval()
            }
          }).catch(fail => {
            this.loading = false
          })
        } else {
          console.log('error submit!!')
          return false
        }
      })
    },
    /** 获取验证码 */
    getVerificationCode() {
      if (this.loginForm.username) {
        sendVerificationCode(this.loginForm.username).then(res => {
          if (res.code === 200) {
            this.changeGetVerificationCodeButton()
            this.buttonDisabled = true
            this.intervalTemp = setInterval(() => this.changeGetVerificationCodeButton(), 1000)
            this.$notify({
              title: '操作通知',
              message: '验证码发送成功',
              tyle: 'success',
              duration: 2000
            })
          } else {
            this.$notify.error({
              title: '操作通知',
              message: res.msg,
              duration: 2000
            })
          }
        })
      } else {
        this.$notify.error({
          title: '操作通知',
          message: '请输入用户名',
          duration: 2000
        })
      }
    },
    /** “获取验证码”按钮文字倒计时 */
    changeGetVerificationCodeButton() {
      this.second -= 1
      // sessionStorage.setItem('second',this.second)
      this.buttonText = this.second + 's后可重试'
      if (this.second <= 0) {
        this.clearGetCodeBtnTextInterval()
      }
    },
    clearGetCodeBtnTextInterval() {
      this.second = 60
      // sessionStorage.setItem('second',this.second)
      this.buttonDisabled = false
      this.buttonText = '获取验证码'
      clearInterval(this.intervalTemp)
    },
    codeInputChange() {
      if (this.loginForm.verificationCode.length > 0 && isNaN(this.loginForm.verificationCode)) {
        if (this.loginForm.verificationCode.length === 1) {
          this.loginForm.verificationCode = ''
        } else {
          this.loginForm.verificationCode = this.loginForm.verificationCode.substring(0, this.loginForm.verificationCode.length - 1)
        }
      }
    }
  }
}
</script>

<style lang="scss">
/* 修复input 背景不协调 和光标变色 */
/* Detail see https://github.com/PanJiaChen/vue-element-admin/pull/927 */

$bg: #283443;
$light_gray: #fff;
$cursor: #fff;

@supports (-webkit-mask: none) and (not (cater-color: $cursor)) {
  .login-container .el-input input {
    color: $cursor;
  }
}

.el-form-item__content {
  height: 41px;
  line-height: 30px;
}

/* reset element-ui css */
.login-container {
  .el-input {
    display: inline-block;
    height: 30px;
    width: 85%;

    input {
      background: transparent;
      border: 0px;
      -webkit-appearance: none;
      border-radius: 0px;
      padding: 12px 5px 12px 15px;
      color: $light_gray;
      height: 30px;
      caret-color: $cursor;

      &:-webkit-autofill {
        box-shadow: 0 0 0px 1000px $bg inset !important;
        -webkit-text-fill-color: $cursor !important;
      }
    }
  }

  .el-form-item {
    border: 1px solid rgba(255, 255, 255, 0.1);
    background: rgba(0, 0, 0, 0.1);
    border-radius: 5px;
    color: #454545;
  }
}
</style>

<style lang="scss" scoped>
$bg: #2d3a4b;
$dark_gray: #889aa4;
$light_gray: #eee;

.login-container {
  min-height: 100%;
  width: 100%;
  background-color: $bg;
  overflow: hidden;

  .login-form {
    position: relative;
    width: 520px;
    max-width: 100%;
    padding: 160px 35px 0;
    margin: 0 auto;
    overflow: hidden;
  }

  .tips {
    font-size: 14px;
    color: #fff;
    margin-bottom: 10px;

    span {
      &:first-of-type {
        margin-right: 16px;
      }
    }
  }

  .svg-container {
    padding: 6px 5px 6px 15px;
    color: $dark_gray;
    vertical-align: middle;
    width: 30px;
    display: inline-block;
  }

  .title-container {
    position: relative;

    .title {
      font-size: 26px;
      color: $light_gray;
      margin: 0px auto 40px auto;
      text-align: center;
      font-weight: bold;
    }

    .languageSelect{
      float: right;
      height: 100%;
      font-size: 18px;
      color: #d8d8d8;
    }
  }

  .show-pwd {
    position: absolute;
    right: 10px;
    top: 7px;
    font-size: 16px;
    color: $dark_gray;
    cursor: pointer;
    user-select: none;
  }
}
</style>
