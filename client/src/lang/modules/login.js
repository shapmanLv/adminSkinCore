export const login_en = {
  login: {
    title: ' Login',
    loginBtn: 'login',
    username: 'username',
    password: 'password',
    captcha: 'captcha',
    getCaptcha: 'get captcha',
    loginSuccessNotify: 'Login Successfully',
    loginError: {
      10001: 'The user does not exist',
      10002: 'This account has been restricted, please contact the administrator',
      10003: 'The account has been frozen due to too many password errors. Please try again later',
      10004: 'The captcha has expired. Please retrieve it again',
      10005: 'Wrong captcha',
      10006: 'Wrong password'
    },
    captchaSuccessNotify: 'Verification code has been sent successfully',
    captchaError: {
      notInputUsername: 'Please input user name',
      10001: 'The user does not exist',
      10002: 'Operation is too frequent, please try again later',
      10003: 'Text message sending failed'
    },
    formRules: {
      username: 'Please input user name',
      password: 'Please input password',
      captcha: {
        none: 'Please input captcha',
        length: 'The captcha format does not match. The length should be 6 bits',
        type: 'The captcha must be a number'
      }
    },
    countdownSuffix: ' second'
  }
}

export const login_zh = {
  login: {
    title: '登录',
    loginBtn: '登录',
    username: '用户名',
    password: '密码',
    captcha: '验证码',
    getCaptcha: '获取验证码',
    loginSuccessNotify: '登录成功',
    loginError: {
      10001: '用户不存在',
      10002: '此账户已被限制使用，请联系管理员',
      10003: '因密码错误次数过多，账号已被冻结，请稍后再试',
      10004: '验证码已过期，请重新获取',
      10005: '验证码错误',
      10006: '密码错误'
    },
    captchaSuccessNotify: '验证码发送成功',
    captchaError: {
      notInputUsername: '请输入用户名',
      10001: '用户不存在',
      10002: '操作太频繁，请稍后再试',
      10003: '短信发送失败'
    },
    formRules: {
      username: '请输入用户名',
      password: '请输入密码',
      captcha: {
        none: '请输入验证码',
        length: '验证码格式不符合，长度应该是6位',
        type: '验证码必须是数字'
      }
    },
    countdownSuffix: 's后可重试'
  }
}
