<template>
  <div class="app-container">
    <div class="filter-container" style="margin-bottom: 20px">
      <el-input
        v-model="listQuery.account"
        placeholder="按照登录名进行搜索"
        style="width: 200px"
        class="filter-item"
        size="small"
        @keyup.enter.native="handleFilter"
      />
      <el-input
        v-model="listQuery.name"
        placeholder="按照名字进行搜索"
        style="width: 200px"
        class="filter-item"
        size="small"
        @keyup.enter.native="handleFilter"
      />
      <el-button-group>
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
          type="primary"
          size="small"
          @click="resetQuery()"
        >
          重置
          <i class="el-icon-refresh" />
        </el-button>
      </el-button-group>

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
      >删除</el-button>
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
      <el-table-column prop="account" label="登录名" width="120px" />
      <el-table-column prop="name" label="名字" width="120px" />
      <el-table-column prop="lastLoginTime" label="最近一次登录时间" width="150px" />
      <el-table-column prop="lastLoginIp" label="最近一次登录IP" width="130px" />
      <el-table-column prop="phoneNumber" label="手机号" width="120px" />
      <el-table-column prop="email" label="邮箱" width="150px" />
      <el-table-column label="角色" min-width="150px">
        <template slot-scope="{ row }">
          <el-tag v-for="item in row.roleInfos" :key="item.Id" style="margin-left:5px">{{ item.name }}</el-tag>
        </template>
      </el-table-column>
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
      :close-on-click-modal="true"
    >
      <el-form
        ref="formData"
        label-width="150px"
        :model="formData"
        :rules="formRules"
      >
        <el-form-item label="名字" prop="name">
          <el-input
            v-model="formData.name"
            autocomplete="off"
            maxlength="15"
            show-word-limit
          />
        </el-form-item>
        <el-form-item v-show="isAddForm" label="登录名" prop="account">
          <el-input
            v-model="formData.account"
            maxlength="25"
            show-word-limit
          />
        </el-form-item>
        <el-form-item label="密码" prop="password">
          <PasswordInput ref="pwdInput" :pwid="pw1" :password.sync="formData.password" :inputplaceholder="passwordPlaceHolder" :pwdmaxlength="20" />
        </el-form-item>
        <el-form-item label="确认密码" prop="passwordAgain">
          <PasswordInput ref="pwdInputAgain" :pwid="pw2" :password.sync="formData.passwordAgain" :pwdmaxlength="20" />
        </el-form-item>
        <el-form-item label="手机号" prop="phoneNumber">
          <el-input
            v-model="formData.phoneNumber"
            maxlength="11"
          />
        </el-form-item>
        <el-form-item label="邮箱" prop="email">
          <el-input
            v-model="formData.email"
            maxlength="50"
          />
        </el-form-item>
        <el-form-item label="角色" prop="roleIds">
          <el-select v-model="formData.roleIds" multiple placeholder="">
            <el-option
              v-for="item in roleList"
              :key="item.id"
              :label="item.name"
              :value="item.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button type="success" @click.native="randomPassword">随机密码</el-button>
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
  GetUserList,
  EditUser,
  AddUser,
  RemoveUser,
  BatchRemoveUser
} from '@/api/user'
import { GetAllRole } from '@/api/role'
import PasswordInput from './components/passwordInput'

export default {
  name: 'User',
  components: { Pagination, PasswordInput },
  directives: { elDragDialog },
  data() {
    const validateName = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入名字'))
      }
    }
    const validateAccount = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        callback(new Error('请输入登录名'))
      }
    }
    const validatePasswordAgain = (rule, value, callback) => {
      if (this.formData.password === value) {
        callback()
      }

      callback(new Error('两次密码不一致'))
    }
    const validatePassword = (rule, value, callback) => {
      if (value) {
        callback()
      } else {
        if (!this.isAddForm) {
          callback()
        }
        callback(new Error('请输入密码'))
      }
    }
    const validatePhoneNumber = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入手机号'))
      }
      if (!(/^1[3456789]\d{9}$/.test(value))) {
        callback(new Error('手机号格式错误'))
      } else {
        callback()
      }
    }
    const validateEmail = (rule, value, callback) => {
      if (value && !(/^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$/.test(value))) {
        callback(new Error('邮箱格式错误'))
      }
      callback()
    }
    return {
      pasArr: ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '_', '-', '$', '%', '&', '@', '+', '!'],
      list: [],
      roleList: [],
      total: 0,
      listLoading: true,
      multipleSelection: [],
      listQuery: {
        page: 1,
        pagesize: 20,
        name: '',
        account: ''
      },
      dialogVisible: false,
      dialogTitle: '', // 编辑/新增
      formData: {
        id: 0,
        account: '',
        name: '',
        password: '',
        passwordAgain: '', // 再次输入密码
        phoneNumber: '',
        email: '',
        roleIds: []
      },
      isAddForm: false,
      formLoading: false,
      formRules: {
        account: [
          { required: true, validator: validateAccount, trigger: 'blur' }
        ],
        name: [
          { required: true, validator: validateName, trigger: 'blur' }
        ],
        password: [
          { required: true, validator: validatePassword, trigger: 'blur' }
        ],
        passwordAgain: [
          { required: true, validator: validatePasswordAgain, trigger: 'blur' }
        ],
        phoneNumber: [
          { required: true, validator: validatePhoneNumber, trigger: 'blur' }
        ],
        email: [
          { validator: validateEmail, trigger: 'blur' }
        ]
      },
      pw1: 'pw1',
      pw2: 'pw2'
    }
  },
  computed: {
    passwordPlaceHolder() {
      if (!this.isAddForm) {
        return '留空，不输入任何值，表示不修改密码'
      } else {
        return ''
      }
    }
  },
  created() {
    this.getList()
    this.getRoleList()
  },
  methods: {
    /** 获取表格数据 */
    getList() {
      this.listLoading = true
      const data = { ...this.listQuery }
      GetUserList(data).then((res) => {
        if (res.code === 200) {
          this.total = res.data.totalCount
          this.list = res.data.userInfos
        } else {
          this.$notify.error({
            title: '操作通知',
            message: '数据加载失败',
            duration: 2000
          })
        }

        setTimeout(() => {
          this.listLoading = false
        }, 300)
      }).catch(res => { this.listLoading = false })
    },
    /** 获取角色列表 */
    getRoleList() {
      GetAllRole().then(res => {
        if (res.code === 200) {
          this.roleList = res.data
        }
      })
    },
    /** 搜索按钮点下，筛选数据 */
    handleFilter() {
      this.listQuery.page = 1 // 搜索的时候，页码都重置为1
      this.getList()
    },
    /** 显示新增表单 */
    handerShowCreateDialog() {
      this.formData.account = ''
      this.formData.name = ''
      this.formData.password = ''
      this.formData.passwordAgain = ''
      this.formData.phoneNumber = ''
      this.formData.email = ''
      this.formData.roleIds = []
      this.dialogTitle = '添加'
      this.isAddForm = true
      this.dialogVisible = true
      setTimeout(() => {
        this.$refs.pwdInputAgain.clear()
        this.$refs.pwdInput.clear()
      }, 100)
    },
    /** 显示修改表单 */
    handleShowUpdateDialog(index) {
      this.formData = {
        ...this.list[index],
        roleIds: this.list[index].roleInfos.map(u => u.id),
        password: '',
        passwordAgain: ''
      }
      this.dialogTitle = '修改'
      this.isAddForm = false
      this.dialogVisible = true
      setTimeout(() => {
        this.$refs.pwdInputAgain.clear()
        this.$refs.pwdInput.clear()
      }, 100)
    },
    /** 删除 */
    handleDelete(id, index) {
      this.$confirm('此操作将永久删除该记录, 是否继续?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        RemoveUser(id).then((res) => {
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
              message: '成功',
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
          console.log(this.formData)
          this.formLoading = true
          this.formData.name = this.trim(this.formData.name)
          this.formData.account = this.trim(this.formData.account)
          if (this.isAddForm) {
            AddUser(this.formData)
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
            EditUser(this.formData)
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
                    message: '成功',
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
    },
    /** 重置查询 */
    resetQuery() {
      this.listQuery.account = ''
      this.listQuery.name = ''
      this.listQuery.roleId = ''
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
        BatchRemoveUser({
          UserIds: this.multipleSelection.map((u) => u.id)
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
    randomPassword() {
      const pasLen = 10
      var password = ''
      var pasArrLen = this.pasArr.length
      for (var i = 0; i < pasLen; i++) {
        var x = Math.floor(Math.random() * pasArrLen)
        password += this.pasArr[x]
      }
      console.log(password)
      this.formData.password = password
      this.formData.passwordAgain = password
      setTimeout(() => {
        this.$refs.pwdInputAgain.setPassword(password)
        this.$refs.pwdInput.setPassword(password)
      }, 10)
    }
  }
}
</script>

<style scoped>
.link-type {
  color: rgb(44, 112, 190);
  cursor: pointer;
}

.el-button-group{
  margin-bottom: 2px;
  position: relative;
}
</style>
