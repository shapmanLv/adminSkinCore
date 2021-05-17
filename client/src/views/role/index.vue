<template>
  <div class="app-container">
    <div class="filter-container" style="margin-bottom: 20px">
      <el-input
        v-model="listQuery.name"
        placeholder="输入角色名"
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
        type="primary"
        icon="el-icon-plus"
        size="small"
        @click="handerShowCreateDialog()"
      >
        添加
      </el-button>
      <el-button
        class="filter-item"
        style="margin-left: 10px"
        type="warning"
        icon="el-icon-delete"
        size="small"
        @click="handlerBatchesDelete()"
      >批量删除</el-button>
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
      <el-table-column prop="name" label="角色" min-width="150px" />
      <el-table-column prop="desc" label="描述" min-width="150px" />
      <el-table-column
        label="操作"
        align="center"
        width="100"
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
      :visible.sync="dialogVisible"
      :title="dialogTitle"
    >
      <el-form
        ref="formData"
        label-width="120px"
        :model="formData"
        :rules="formRules"
        style="height: 500px;overflow-y: scroll;"
      >
        <el-form-item label="角色名" prop="name">
          <el-input
            v-model="formData.name"
            maxlength="20"
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
        <div style="clear:both" />
        <el-form-item label="接口绑定" style="height: 320px" prop="authorizeApiIdIds">
          <el-transfer
            v-model="temp"
            style="text-align: left; display: inline-block"
            :props="{
              key: 'id',
              label: 'routerPath'
            }"
            :titles="['已绑定接口', '未绑定接口']"
            filterable
            size="mini"
            :render-content="renderFunc"
            :filter-method="transferFilterMethod"
            filter-placeholder="请输入接口路径"
            :format="{
              noChecked: '${total}',
              hasChecked: '${checked}/${total}'}"
            :data="authorizeApiList"
          />
        </el-form-item>
        <el-form-item label="菜单绑定" prop="menuIds">
          <el-tree
            ref="tree"
            :props="{label: 'name'}"
            :data="menuTree"
            node-key="id"
            highlight-current
            show-checkbox
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
  GetRoleList,
  EditRole,
  AddRole,
  RemoveRole,
  BatchRemoveRole
} from '@/api/role'
import { GetAllAuthorizeApi } from '@/api/authorizeApi'
import { GetMenuTree } from '@/api/menu'

export default {
  name: 'Role',
  components: { Pagination },
  directives: { elDragDialog },
  data() {
    const validateName = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入角色名'))
      }
    }
    const validateAuthorizeApiIdIds = (rule, value, callback) => {
      if (this.temp !== null && this.temp.length > 0) {
        callback()
      } else {
        callback(new Error('请为该角色分配接口'))
      }
    }
    return {
      list: [],
      authorizeApiList: [],
      menuTree: [],
      temp: [],
      total: 0,
      listLoading: true,
      multipleSelection: [],
      listQuery: {
        page: 1,
        pagesize: 20,
        name: ''
      },
      dialogVisible: false,
      dialogTitle: '', // 编辑/新增
      formData: {
        id: 0,
        name: '',
        desc: '',
        authorizeApiIdIds: [],
        menuIds: []
      },
      isAddForm: false,
      formLoading: false,
      formRules: {
        name: [
          { required: true, validator: validateName, trigger: 'blur' }
        ],
        authorizeApiIdIds: [
          { required: true, validator: validateAuthorizeApiIdIds, trigger: 'change' }
        ]
      }
    }
  },
  created() {
    this.getList()
    this.getAuthorizeApiList()
    this.getMenu()
  },
  methods: {
    /** 获取表格数据 */
    getList() {
      this.listLoading = true
      const data = { ...this.listQuery }
      GetRoleList(data).then((res) => {
        if (res.code === 200) {
          this.total = res.data.totalCount
          this.list = res.data.roleInfos
        } else {
          this.$notify.error({
            title: '操作错误',
            message: '加载数据失败',
            duration: 2000
          })
        }

        setTimeout(() => {
          this.listLoading = false
        }, 300)
      }).catch(res => { this.listLoading = false })
    },
    /** 获取所有接口 */
    getAuthorizeApiList() {
      GetAllAuthorizeApi().then(res => {
        if (res.code === 200) {
          this.authorizeApiList = res.data
        }
      })
    },
    /** 获取菜单树 */
    getMenu() {
      GetMenuTree().then(res => {
        if (res.code === 200) {
          this.menuTree = res.data
        }
      })
    },
    /** 接口分配里面，设置悬浮显示 */
    renderFunc(h, option) {
      return <span title={option.routerPath}>{ option.routerPath }</span>
    },
    /** 搜索按钮点下，筛选数据 */
    handleFilter() {
      this.listQuery.page = 1 // 搜索的时候，页码都重置为1
      this.getList()
    },
    /** 接口分配的穿梭框中，数据筛选的处理函数 */
    transferFilterMethod(query, item) {
      return item.routerPath.indexOf(query) > -1
    },
    /** 显示新增表单 */
    handerShowCreateDialog() {
      this.formData.name = '' // 上面这一行重置表单经常不一定能成功，所以又手动的改
      this.formData.desc = ''
      this.formData.authorizeApiIdIds = []
      this.formData.menuIds = []
      this.dialogTitle = '添加'
      this.dialogVisible = true
      this.isAddForm = true
      this.$nextTick(function() {
        this.$refs.tree.setCheckedKeys([])
        this.temp = []
      })
    },
    /** 显示修改表单 */
    handleShowUpdateDialog(i) {
      this.formData = { ...this.list[i] }
      this.formData.authorizeApiIdIds = []
      this.temp = this.list[i].authorizeApiInfos.map(u => u.id)

      this.dialogTitle = '修改'
      this.dialogVisible = true
      this.isAddForm = false
      this.$nextTick(function() {
        this.$refs.tree.setCheckedKeys(this.list[i].menuIds)
      })
    },
    /** 删除 */
    handleDelete(id, index) {
      this.$confirm('此操作将永久删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        RemoveRole(id).then((res) => {
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
      this.formData.menuIds = this.$refs.tree.getCheckedKeys()
      this.formData.authorizeApiIdIds = this.temp
      this.$refs[formname].validate((valid) => {
        if (valid) {
          this.formLoading = true
          this.formData.name = this.trim(this.formData.name)
          this.formData.desc = this.trim(this.formData.desc)
          if (this.isAddForm) {
            AddRole(this.formData)
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
            EditRole(this.formData)
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
        } else {
          console.log('未通过表单规则')
        }
      })
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
        BatchRemoveRole({
          RoleIds: this.multipleSelection.map((u) => u.id)
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
    }
  }
}
</script>

<style scoped>
>>>.el-transfer-panel__body {
    height: 266px;
}

>>>.el-transfer-panel__body .el-input{
      width: 90%
    }

.link-type {
  color: rgb(44, 112, 190);
  cursor: pointer;
}

  >>>.el-dialog{
    min-width: 1020px;
    min-height: 750px;
  }

  >>>.el-transfer-panel {
    width: 328px;
  }

</style>
