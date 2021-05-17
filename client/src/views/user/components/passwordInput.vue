<template>
  <el-input v-if="pwdmaxlength > 0" :id="pwid" v-model="pwd" name="myPasswordInput" :placeholder="inputplaceholder" :maxlength="pwdmaxlength" @input="pwdChange" />
  <el-input v-else :id="pwid" :value="pwd" name="myPasswordInput" :placeholder="inputplaceholder" @input="pwdChange" />
</template>

<script>
export default {
  props: {
    password: {
      required: true,
      type: String
    },
    pwid: {
      required: true,
      type: String,
      default: 'pw'
    },
    inputplaceholder: {
      required: false,
      type: String,
      default: ''
    },
    pwdmaxlength: {
      required: false,
      type: Number,
      default: 0
    }
  },
  data() {
    return {
      pwd: '',
      pwdTemp: ''
    }
  },
  methods: {
    pwdChange() {
      var that = document.getElementById(this.pwid)
      if (this.pwd) {
        var afterProcessingShowStr = '' // 处理过后的用于显示的字符串
        const userInputContentChars = this.pwd.split('') // 用户输入的字符
        const cacheContentChars = this.pwdTemp.split('') // 保存到缓存的密码
        var password = '' // 最终的密码
        var i = 0

        var havesetCaretPositionIndex = 0 // 需要设置光标位置的字符索引
        for (let index = 0; index < userInputContentChars.length; index++) {
          const element = userInputContentChars[index]
          if (element === '•') {
            afterProcessingShowStr += '•'
            password += cacheContentChars[i]
            i++
          } else {
            afterProcessingShowStr += '•'
            password += element
            havesetCaretPositionIndex = index + 1
          }
        }
        // console.log('当前光标的位置为：' + this.getCursortPosition(that))
        this.pwdTemp = password
        this.pwd = afterProcessingShowStr

        if (havesetCaretPositionIndex !== 0) {
          setTimeout(() => {
            this.setCaretPosition(that, havesetCaretPositionIndex)
            // console.log('设置完之后，光标的位置为：' + this.getCursortPosition(that))
          }, 5)
        }
        console.log(this.pwdTemp)
      } else {
        this.pwdTemp = ''
      }
      this.$emit('update:password', this.pwdTemp)
    },
    // 设置光标位置
    setCaretPosition(ctrl, pos) {
      if (ctrl.setSelectionRange) {
        ctrl.focus()
        ctrl.setSelectionRange(pos, pos)
      } else if (ctrl.createTextRange) {
        var range = ctrl.createTextRange()
        console.log(range)
        range.collapse(true)
        range.moveEnd('character', pos)
        range.moveStart('character', pos)
        range.select()
      }
    },
    getCursortPosition(ctrl) {
      var CaretPos = 0 // IE Support
      if (document.selection) {
        ctrl.focus()
        var Sel = document.selection.createRange()
        Sel.moveStart('character', -ctrl.value.length)
        CaretPos = Sel.text.length
      } else if (ctrl.selectionStart || ctrl.selectionStart === '0') { // Firefox support
        CaretPos = ctrl.selectionStart
      } else {
        console.log(ctrl)
      }
      return CaretPos
    },
    clear() {
      this.pwd = ''
      this.pwdTemp = ''
    },
    setPassword(pwd) {
      this.pwdTemp = pwd
      this.pwd = pwd
      // this.pwd = ''
      // for (let index = 0; index < pwd.length; index++) {
      //   this.pwd += '•'
      // }
    }
  }
}
</script>
