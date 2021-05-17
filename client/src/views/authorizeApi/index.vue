<template>
  <div class="app-container">
    <div class="filter-container" style="margin-bottom: 20px">
      <el-input
        v-model="listQuery.routerPath"
        placeholder="输入接口路径"
        style="width: 200px"
        class="filter-item"
        size="small"
        @keyup.enter.native="handleFilter"
      />
      <el-button
        class="filter-item"
        type="primary"
        icon="el-icon-search"
        size="small"
        @click="handleFilter()"
      >
        搜索
      </el-button>
      <el-button
        class="filter-item"
        style="margin-left: 10px"
        type="warning"
        icon="el-icon-delete"
        size="small"
        @click="handlerBatchesDelete()"
      >批量删除</el-button>
      <!-- <el-button
        class="filter-item"
        style="margin-left: 10px"
        type="primary"
        icon="el-icon-plus"
        size="small"
        @click="handerShowCreateDialog()"
      >
        {{ $t('btn.add') }}
      </el-button>

      <el-button
        class="filter-item"
        style="margin-left: 10px"
        type="primary"
        icon="el-icon-plus"
        size="small"
        @click="scanAddAdd()"
      >
        {{ $t('btn.scanAdd') }}
      </el-button> -->
    </div>

    <el-table
      v-loading="listLoading"
      :data="list"
      border
      fit
      highlight-current-row
      style="width: 100%"
      :header-cell-style="{background:'#F3F4F7',color:'#555'}"
      size="small"
      @selection-change="handleSelectionChange"
    >
      <el-table-column type="selection" width="50" />
      <el-table-column label="序号" align="center" width="80">
        <template slot-scope="{ $index }">
          <span>{{
            $index + (listQuery.page - 1) * listQuery.pagesize + 1
          }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="routerPath" label="接口路径" min-width="150px" />
      <el-table-column prop="desc" label="描述" min-width="150px" />
      <el-table-column
        label="操作"
        align="center"
        width="230"
        fixed="right"
        class-name="small-padding fixed-width"
      >
        <template slot-scope="{ row, $index }">
          <el-button
            type="primary"
            icon="el-icon-edit"
            size="small"
            circle
            @click="handleShowUpdateDialog($index)"
          />
          <el-button
            type="danger"
            icon="el-icon-delete"
            size="small"
            circle
            @click="handleDelete(row.id, $index)"
          />
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="listQuery.page"
      :limit.sync="listQuery.pagesize"
      @pagination="getList()"
    />
    <el-dialog
      v-el-drag-dialog
      :visible.sync="dialogVisible"
      :title="dialogTitle"
      :close-on-click-modal="true"
      @dragDialog="handleDrag"
    >
      <el-form
        ref="formData"
        label-width="80px"
        :model="formData"
        :rules="formRules"
      >
        <el-form-item label="接口路径" prop="routerPath">
          <el-input
            v-model="formData.routerPath"
            :readonly="true"
            type="textarea"
            maxlength="100"
            show-word-limit
          />
        </el-form-item>
        <el-form-item label="描述" prop="desc">
          <el-input
            v-model="formData.desc"
            type="textarea"
            maxlength="100"
            show-word-limit
          />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click.native="closeFormDialog">取消</el-button>
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
import Pagination from '@/components/Pagination' // secondary package based on el-pagination
import elDragDialog from '@/directive/el-drag-dialog'
import {
  GetAuthorizeApiList,
  EditAuthorizeApi,
  AddAuthorizeApi,
  RemoveAuthorizeApi,
  BatchRemoveAuthorizeApi,
  BuildAuthorizeApis
} from '@/api/authorizeApi'

export default {
  name: 'AuthorizeApi',
  components: { Pagination },
  directives: { elDragDialog },
  data() {
    const routerPathValidate = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入接口路径'))
      }
    }
    return {
      list: [],
      total: 0,
      listLoading: true,
      multipleSelection: [],
      listQuery: {
        page: 1,
        pagesize: 20,
        routerPath: ''
      },
      dialogVisible: false,
      dialogTitle: '', // 编辑/新增
      formData: {
        id: 0,
        routerPath: '',
        desc: ''
      },
      isAddForm: false,
      formLoading: false,
      formRules: {
        routerPath: [
          { required: true, validator: routerPathValidate, trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.getList()
  },
  methods: {
    /** 获取表格数据 */
    getList() {
      this.listLoading = true

      let path = ''
      if (this.listQuery.routerPath !== '') {
        const strArray = this.listQuery.routerPath.split('/')
        path = strArray.join('-')
      }
      const data = { ...this.listQuery }
      data.routerPath = path
      GetAuthorizeApiList(data).then((res) => {
        if (res.code === 200) {
          this.total = res.data.totalCount
          this.list = res.data.authorizeApiInfos
        } else {
          this.$notify.error({
            title: '操作错误',
            message: '加载数据失败',
            duration: 2000
          })

          this.listLoading = false
        }

        setTimeout(() => {
          this.listLoading = false
        }, 300)
      }).catch(res => { this.listLoading = false })
    },
    /** 搜索按钮点下，筛选数据 */
    handleFilter() {
      this.listQuery.page = 1 // 搜索的时候，页码都重置为1
      this.getList()
    },
    /** 显示新增表单 */
    handerShowCreateDialog() {
      if (this.$refs.formData !== undefined) this.$refs.formData.resetFields()
      this.formData.routerPath = '' // 上面这一行重置表单经常不一定能成功，所以又手动的改
      this.formData.desc = ''
      this.dialogTitle = '添加'
      this.isAddForm = true
      this.dialogVisible = true
    },
    /** 显示修改表单 */
    handleShowUpdateDialog(index) {
      this.formData = { ...this.list[index] }
      this.dialogTitle = '修改'
      this.isAddForm = false
      this.dialogVisible = true
    },
    /** 删除 */
    handleDelete(id, index) {
      this.$confirm('此操作将永久删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        RemoveAuthorizeApi(id).then((res) => {
          if (res.code === 200) {
            this.$notify({
              title: '操作通知',
              message: '成功',
              type: 'success',
              duration: 2000
            })
            this.list.splice(index, 1)
          } else {
            this.$notify.error({
              title: '操作通知',
              message: res.msg,
              duration: 2000
            })
          }
        })
      })
    },
    /** 表单操作中，取消按钮的事件处理逻辑 */
    closeFormDialog() {
      this.dialogVisible = false
    },
    /** 提交 新增/修改表单 */
    submit(formname) {
      this.$refs[formname].validate((valid) => {
        if (valid) {
          this.formLoading = true
          this.formData.routerPath = this.trim(this.formData.routerPath)
          this.formData.desc = this.trim(this.formData.desc)
          if (this.isAddForm) {
            AddAuthorizeApi(this.formData)
              .then((res) => {
                if (res.code === 200) {
                  this.$notify({
                    title: '操作通知',
                    message: '成功',
                    type: 'success',
                    duration: 2000
                  })
                  this.listQuery.page = 1
                  this.getList()
                  this.dialogVisible = false
                } else {
                  this.$notify.error({
                    title: '操作通知',
                    message: res.msg,
                    duration: 2000
                  })
                }
                this.formLoading = false
              })
              .catch((res) => (this.formLoading = false))
          } else {
            EditAuthorizeApi(this.formData)
              .then((res) => {
                if (res.code === 200) {
                  this.$notify({
                    title: '操作通知',
                    message: '成功',
                    type: 'success',
                    duration: 2000
                  })
                  this.getList()
                  this.dialogVisible = false
                } else {
                  this.$notify.error({
                    title: '操作通知',
                    message: res.msg,
                    duration: 2000
                  })
                }
                this.formLoading = false
              })
              .catch((res) => (this.formLoading = false))
          }
        }
      })
    },
    /** 可拖拽的Dialog所必须的函数 */
    handleDrag() {
      this.$refs.select.blur()
    },
    /** 表格左侧复选框选中状态改变 */
    handleSelectionChange(val) {
      this.multipleSelection = val
      console.log(val)
      const arr = val.map((i) => i.id)
      console.log('arr:' + arr)
    },
    /** 批量删除 */
    handlerBatchesDelete() {
      if (this.multipleSelection.length === 0) {
        this.$notify.error({
          title: '操作通知',
          message: '未选中任何记录',
          duration: 2000
        })
        return
      }
      this.$confirm('此操作将永久删除这些记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        BatchRemoveAuthorizeApi({
          authorizeApiIds: this.multipleSelection.map((u) => u.id)
        }).then((res) => {
          if (res.code === 200) {
            this.$notify({
              title: '操作通知',
              message: '成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
            this.multipleSelection = []
          } else {
            this.$notify.error({
              title: '操作通知',
              message: res.msg,
              duration: 2000
            })
          }
        })
      })
    },
    scanAddAdd() {
      this.$confirm('是否要全局扫描所有服务端接口，搜集需授权才能使用的Api，并记录到数据库中？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info'
      }).then(() => {
        const loading = this.$loading({
          lock: true,
          text: 'Loading',
          spinner: 'el-icon-loading',
          background: 'rgba(0, 0, 0, 0.7)'
        })
        BuildAuthorizeApis().then(res => {
          loading.close()
          if (res.code === 200) {
            this.$notify({
              title: '操作通知',
              message: '成功',
              type: 'success',
              duration: 2000
            })
            this.getList()
          } else {
            this.$notify.error({
              title: '操作通知',
              message: res.msg,
              duration: 2000
            })
          }
        }).catch(res => loading.close())
      })
    }
  }
}
</script>

<style scoped>
.link-type {
  color: rgb(44, 112, 190);
  cursor: pointer;
}
</style>
