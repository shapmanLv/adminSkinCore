<template>
  <div>
    <h3>这个页面是测试是否有接口权限，点击按钮发送请求试试</h3>
    <el-button
      type="primary"
      @click="send()"
    >
      点击请求接口
    </el-button>
    <h1>当没有为用户 test 设置 api test 角色时，点击请求接口会被跳转至 401 页面；同理，当他有这个角色的时候，就能成功请求了。</h1>
    <hr>
    <el-button
      type="primary"
      @click="redirect()"
    >
      点击跳转至 test2 页面
    </el-button>
    <h1>如果没有为用户 test 设置 api test 角色，点击跳转会出现404页面。只有他有这个角色的时候，才既能成功跳转，并且也能在侧边栏菜单看到test2页面入口</h1>
  </div>
</template>
<script>
import { test } from '@/api/systemtest'

export default {
  name: 'Test1',
  methods: {
    send() {
      test().then(res => {
        confirm('请求成功')
      })
    },
    redirect() {
      this.$router.push('/systemtest/test2')
    }
  }
}
</script>
