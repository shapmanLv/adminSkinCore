<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
import $ from 'jquery'

export default {
  name: 'App',
  created() {
    $.getScript('https://pv.sohu.com/cityjson?ie=utf-8')
      .done(function(script, textStatus, jqxhr) {
        console.log('访问搜狐接口获取IP结果：')
        console.log(returnCitySN.cip)// 440306
        console.log(textStatus)// success
        console.log(jqxhr.status)// 200

        localStorage.setItem('ip', returnCitySN.cip)
      })
      .fail(function() {
        console.log('访问搜狐结果获取IP错误')
      })
  }
}

</script>
<style>
 /* 解决element-ui的table表格控件表头与内容列不对齐问题 */
  .el-table th.gutter{
    display: table-cell!important;
  }
</style>
